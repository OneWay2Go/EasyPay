using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TgRobuxBot.Models;
using System.Text.Json;

namespace TgRobuxBot.Services;

public class UpdateHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(IServiceProvider serviceProvider, ILogger<UpdateHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                await HandleMessageAsync(botClient, update.Message, dbContext, cancellationToken);
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await HandleCallbackQueryAsync(botClient, update.CallbackQuery, dbContext, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update");
        }
    }

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (message.Chat.Type == ChatType.Private)
        {
            await HandlePrivateMessageAsync(botClient, message, dbContext, cancellationToken);
        }
        else if (message.Chat.Type == ChatType.Supergroup || message.Chat.Type == ChatType.Group)
        {
            await HandleGroupMessageAsync(botClient, message, dbContext, cancellationToken);
        }
    }
    
    private async Task HandleGroupMessageAsync(ITelegramBotClient botClient, Message message, ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(message.Text)) return;
        
        if (message.Text.StartsWith("/start"))
        {
            var botUser = await botClient.GetMe(cancellationToken);
            string botUsername = botUser.Username ?? "Bot";
            
            var inlineKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithUrl("🤖 Xarid qilish", $"https://t.me/{botUsername}?start=buy")
            );

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Assalomu alaykum, {message.From?.FirstName}! 👋\nRobux, Telegram Stars yoki Premium sotib olish uchun botga o'ting:",
                replyParameters: message.MessageId,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task HandlePrivateMessageAsync(ITelegramBotClient botClient, Message message, ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == message.From!.Id, cancellationToken);
        if (user == null)
        {
            user = new AppUser
            {
                TelegramId = message.From!.Id,
                Username = message.From.Username,
                FirstName = message.From.FirstName,
                State = "Idle"
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // --- REVIEW COLLECTION HANDLING ---
        if (user.State.StartsWith("AwaitingReview_"))
        {
            if (string.IsNullOrEmpty(message.Text)) return;
            
            var orderIdStr = user.State.Split('_')[1];
            if (!int.TryParse(orderIdStr, out int orderId)) return;

            var order = await dbContext.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
            if (order != null)
            {
                var config = _serviceProvider.GetRequiredService<IConfiguration>();
                string? reviewGroupIdStr = config["ReviewsGroupId"];
                string? reviewTopicIdStr = config["ReviewsTopicId"];

                if (long.TryParse(reviewGroupIdStr, out long groupId) && int.TryParse(reviewTopicIdStr, out int topicId))
                {
                    string productName = order.Product.Name;
                    if (productName == "Telegram Stars (Maxsus)" && !string.IsNullOrEmpty(order.AdditionalInfo))
                    {
                        // Try to find the line that says "Maxsus Summa" or just use a generic name
                        productName = "Telegram Stars";
                    }

                    string reviewMsg = $"🌟 <b>Yangi Isbot</b> 🌟\n\n" +
                                       $"👤 <b>Mijoz:</b> {message.From?.FirstName} (@{message.From?.Username})\n" +
                                       $"🛍 <b>Xarid:</b> {productName}\n" +
                                       $"💬 <b>Fikr:</b> \"{message.Text}\"";

                    await botClient.SendMessage(
                        chatId: groupId,
                        messageThreadId: topicId,
                        text: reviewMsg,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
                }
                
                user.State = "Idle";
                await dbContext.SaveChangesAsync(cancellationToken);
                
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Fikringiz uchun katta rahmat! Sizning fikringiz biz uchun juda muhim. 😊",
                    cancellationToken: cancellationToken
                );
            }
            return;
        }

        if (message.Text?.StartsWith("/start") == true)
        {
            user.State = "Idle";
            user.TempData = null;
            await dbContext.SaveChangesAsync(cancellationToken);
            await ShowMainMenuAsync(botClient, message.Chat.Id, cancellationToken);
            return;
        }

        if (message.Text?.StartsWith("/my_id") == true)
        {
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Sizning ID: {message.From!.Id}\nBuni configda 'AdminId' qismi uchun ishlashingiz mumkin.",
                cancellationToken: cancellationToken
            );
            return;
        }

        // --- STATE MACHINE HANDLING ---
        if (!string.IsNullOrEmpty(message.Text))
        {
            var text = message.Text;
            var session = string.IsNullOrEmpty(user.TempData) ? new OrderSessionData() : JsonSerializer.Deserialize<OrderSessionData>(user.TempData);

            if (user.State == "AwaitingStarsAmount")
            {
                if (int.TryParse(text, out int stars) && stars >= 50 && stars <= 10000)
                {
                    decimal price = stars * 240m;
                    session!.CustomPriceUZS = price;
                    session.ProductName = $"{stars} Telegram Stars";
                    
                    user.State = "AwaitingTelegramUsername";
                    user.TempData = JsonSerializer.Serialize(session);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    if (stars >= 1000)
                    {
                        await botClient.SendMessage(
                            chatId: message.Chat.Id,
                            text: "🎉 🎉 🎉\nSiz katta miqdorda Stars xarid qilyapsiz!\nChegirma olish uchun xariddan oldin admin bilan bog'lanishingiz mumkin: @wunego",
                            cancellationToken: cancellationToken
                        );
                    }

                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Yaxshi! Endi Stars yuboriladigan Telegram Username ni kiriting (masalan: @username):",
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Noto'g'ri raqam. Iltimos, 50 dan 10.000 gacha bo'lgan butun son kiriting:",
                        cancellationToken: cancellationToken
                    );
                }
                return;
            }
            
            if (user.State == "AwaitingTelegramUsername")
            {
                session!.TargetTelegramUsername = text;
                user.State = "AwaitingDataConfirmation";
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await ShowDataConfirmationAsync(botClient, message.Chat.Id, session, cancellationToken);
                return;
            }

            if (user.State == "AwaitingPremiumPhone")
            {
                session!.PhoneNumber = text;
                user.State = "AwaitingDataConfirmation";
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await ShowDataConfirmationAsync(botClient, message.Chat.Id, session, cancellationToken);
                return;
            }

            if (user.State == "AwaitingRobloxUsername")
            {
                session!.RobloxUsername = text;
                user.State = "AwaitingRobloxPassword";
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Endi Roblox parolingizni kiriting:\n\nℹ️ Parolingiz qat'iy himoyalangan tarzda saqlanadi va xariddan so'ng darhol o'chiriladi.",
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (user.State == "AwaitingRobloxPassword")
            {
                using var scope = _serviceProvider.CreateScope();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                string encKey = config["EncryptionKey"] ?? "12345678901234567890123456789012";

                session!.RobloxPasswordEncrypted = EncryptionHelper.Encrypt(text, encKey);
                // Instead of asking for the text code, we just ask a Yes/No question
                user.State = "AwaitingRoblox2FAQuery"; 
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Ha", "has2fa_yes") },
                    new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Yo'q", "has2fa_no") }
                });

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Robloxda 2FA (Ikki bosqichli tasdiqlash) ulanganmi?\n(Agar ulangan bo'lsa, xariddan so'ng kodni olish uchun admin siz bilan alohida bog'lanadi)",
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        // --- PAYMENT PHOTO HANDLING ---
        if (message.Photo != null && message.Photo.Length > 0)
        {
            if (user?.State == "AwaitingPayment")
            {
                var session = string.IsNullOrEmpty(user.TempData) ? null : JsonSerializer.Deserialize<OrderSessionData>(user.TempData);
                if (session == null) return;

                var product = await dbContext.Products.FindAsync(new object[] { session.ProductId }, cancellationToken);
                if (product == null) return;

                var photoFileId = message.Photo.Last().FileId;

                // Move data to DB Order
                using var scope = _serviceProvider.CreateScope();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                string encKey = config["EncryptionKey"] ?? "12345678901234567890123456789012";

                string decryptedPassword = session.RobloxPasswordEncrypted != null ? EncryptionHelper.Decrypt(session.RobloxPasswordEncrypted, encKey) : "N/A";
                

                decimal finalPrice = session.CustomPriceUZS > 0 ? session.CustomPriceUZS : product.PriceUZS;
                string finalName = session.ProductName ?? product.Name;

                // Build structured additionalInfo per category
                string additionalInfo = "";
                string displayProductName = finalName;
                
                if (product.Category == "Robux")
                {
                    additionalInfo = $"👤 Roblox Username: <code>{session.RobloxUsername}</code>\n" +
                                     $"🔑 Parol: <code>{decryptedPassword}</code>\n" +
                                     $"🔐 2FA: {session.Roblox2FA}";
                }
                else if (product.Category == "TelegramStars")
                {
                    displayProductName = session.ProductName ?? finalName;
                    additionalInfo = $"⭐ Miqdor: {session.ProductName}\n" +
                                     $"👤 Stars Yuboriladigan Username: {session.TargetTelegramUsername}\n" +
                                     $"💰 Narxi: {session.CustomPriceUZS:N0} UZS";
                }
                else if (product.Category == "TelegramPremium" && product.Name.Contains("Gift"))
                {
                    additionalInfo = $"👤 Gift Username: {session.TargetTelegramUsername}";
                }
                else if (product.Category == "TelegramPremium")
                {
                    additionalInfo = $"📱 Telefon Raqam: {session.PhoneNumber}";
                }

                var order = new Order
                {
                    UserId = user.TelegramId,
                    ProductId = product.Id,
                    Status = "PendingConfirmation",
                    ReceiptFileId = photoFileId,
                    AdditionalInfo = additionalInfo,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Orders.Add(order);
                
                user.State = "Idle";
                user.TempData = null; // Clear sensitive data
                
                await dbContext.SaveChangesAsync(cancellationToken);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "✅ To'lov cheki qabul qilindi. Buyurtmangiz admin tomonidan 30 daqiqadan 1 soatgacha bo'lgan vaqt ichida bajariladi.\n\n" +
                          "⚠️ Iltimos, agar 1 soatdan ko'p vaqt o'tmagan bo'lsa, adminni bezovta qilmang. Agar biror muammo bo'lsa, admin o'zi sizga yozadi.\n\n" +
                          "👤 Admin: @wunego",
                    cancellationToken: cancellationToken
                );

                // Notify admin
                long adminId = 0;
                if (long.TryParse(config["AdminId"], out long parsedAdmin)) adminId = parsedAdmin;
                else adminId = 8125470010;

                var adminKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Tasdiqlash", $"approve_{order.Id}"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Rad etish", $"reject_{order.Id}")
                    }
                });

                string adminMsg = $"Yangi buyurtma!\n\n" +
                                  $"Foydalanuvchi: @{user.Username} (ID: {user.TelegramId})\n" +
                                  $"Mahsulot: {finalName} ({product.Category})\n" +
                                  $"Summa: {finalPrice:N0} UZS\n\n" +
                                  $"Ma'lumotlar:\n{additionalInfo}";

                _logger.LogInformation("Sending notification to admin ID: {AdminId}", adminId);
                await botClient.SendPhoto(
                    chatId: adminId,
                    photo: photoFileId,
                    caption: adminMsg,
                    replyMarkup: adminKeyboard,
                    cancellationToken: cancellationToken
                );
            }
        }
    }

    private async Task ShowMainMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Robux", "category_Robux") },
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Telegram Stars", "category_TelegramStars") },
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Telegram Premium", "category_TelegramPremium") }
        });

        await botClient.SendMessage(
            chatId: chatId,
            text: "Assalomu alaykum! Robux yoki Telegram Stars xarid qilish uchun menyudan tanlang.",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task ShowDataConfirmationAsync(ITelegramBotClient botClient, long chatId, OrderSessionData session, CancellationToken cancellationToken)
    {
        string info = $"Mahsulot: {session.ProductName}\n\nSiz kiritgan ma'lumotlar:\n";
        
        if (!string.IsNullOrEmpty(session.RobloxUsername))
            info += $"- Roblox User: {session.RobloxUsername}\n- Parol: ••••••••\n- 2FA/Kod: {session.Roblox2FA}\n";
        if (!string.IsNullOrEmpty(session.TargetTelegramUsername))
            info += $"- Telegram Username: {session.TargetTelegramUsername}\n";
        if (!string.IsNullOrEmpty(session.PhoneNumber))
            info += $"- Telefon Raqam: {session.PhoneNumber}\n";

        info += "\nMa'lumotlar to'g'rimi? Davom etishni tasdiqlang.";

        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Ha, to'g'ri", "confirm_data") },
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Qaytadan kiritish", "cancel_buy") }
        });

        await botClient.SendMessage(
            chatId: chatId,
            text: info,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data == null) return;

        var data = callbackQuery.Data;
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == callbackQuery.From.Id, cancellationToken);
        if (user == null) return;

        if (data.StartsWith("category_") || data == "robux_page_1" || data == "robux_page_2")
        {
            string category = data.StartsWith("category_") ? data.Split('_')[1] : "Robux";

            if (category == "TelegramStars")
            {
                user.State = "AwaitingStarsAmount";
                var session = new OrderSessionData { ProductId = 21 };
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);

                await botClient.EditMessageText(
                    chatId: callbackQuery.Message!.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: "Telegram Stars olish uchun qancha Stars kerakligini raqamda yozib yuboring.\n(Eng kami 50, Eng ko'pi 10.000)",
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back_main")
                    ),
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (category == "Robux")
            {
                int page = data == "robux_page_2" ? 2 : 1;
                var allRobux = await dbContext.Products
                    .Where(p => p.Category == "Robux")
                    .OrderBy(p => p.PriceUZS)
                    .ToListAsync(cancellationToken);

                // Split: Page 1 = first 12 products, Page 2 = rest
                const int pageSize = 12;
                var pageProducts = page == 1
                    ? allRobux.Take(pageSize).ToList()
                    : allRobux.Skip(pageSize).ToList();

                var buttons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                for (int i = 0; i < pageProducts.Count; i += 2)
                {
                    var row = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        $"🎮 {pageProducts[i].Name} — {pageProducts[i].PriceUZS:N0} so'm", $"startbuy_{pageProducts[i].Id}"));
                    if (i + 1 < pageProducts.Count)
                        row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            $"🎮 {pageProducts[i+1].Name} — {pageProducts[i+1].PriceUZS:N0} so'm", $"startbuy_{pageProducts[i+1].Id}"));
                    buttons.Add(row);
                }

                // Navigation row
                var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                if (page == 1)
                {
                    navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back_main"));
                    navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Keyingi ➡️", "robux_page_2"));
                }
                else
                {
                    navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Oldingi", "robux_page_1"));
                    navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Bosh menyu", "back_main"));
                }
                buttons.Add(navRow);

                await botClient.EditMessageText(
                    chatId: callbackQuery.Message!.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: $"🎮 Robux ({page}/2-sahifa). Xarid qilmoqchi bo'lgan paketni tanlang:",
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: cancellationToken
                );
                return;
            }

            // Premium (single page)
            var products = await dbContext.Products.Where(p => p.Category == category).OrderBy(p => p.PriceUZS).ToListAsync(cancellationToken);
            var premiumButtons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
            foreach (var p in products)
            {
                premiumButtons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"💎 {p.Name} — {p.PriceUZS:N0} so'm", $"startbuy_{p.Id}")
                });
            }
            premiumButtons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back_main")
            });

            await botClient.EditMessageText(
                chatId: callbackQuery.Message!.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                text: "Telegram Premium bo'limidan tanlang:",
                replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(premiumButtons),
                cancellationToken: cancellationToken
            );
        }
        else if (data == "back_main" || data == "cancel_buy")
        {
            user.State = "Idle";
            user.TempData = null;
            await dbContext.SaveChangesAsync(cancellationToken);
            
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Robux", "category_Robux") },
                new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Telegram Stars", "category_TelegramStars") },
                new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Telegram Premium", "category_TelegramPremium") }
            });

            await botClient.EditMessageText(
                chatId: callbackQuery.Message!.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                text: "Menyuga qaytdingiz. Nima xarid qilasiz?",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }
        else if (data.StartsWith("startbuy_"))
        {
            if (int.TryParse(data.Split('_')[1], out int productId))
            {
                var product = await dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
                if (product != null)
                {
                    var session = new OrderSessionData { ProductId = product.Id, ProductName = product.Name };
                    
                    if (product.Category == "Robux")
                    {
                        user.State = "AwaitingRobloxUsername";
                        await botClient.EditMessageText(
                            chatId: callbackQuery.Message!.Chat.Id,
                            messageId: callbackQuery.Message.MessageId,
                            text: $"Tanlandi: {product.Name}\n\nIltimos, Roblox Username (Login) ni kiriting:",
                            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "cancel_buy")),
                            cancellationToken: cancellationToken
                        );
                    }
                    else if (product.Category == "TelegramPremium")
                    {
                        if (product.Name.Contains("Gift"))
                        {
                            user.State = "AwaitingTelegramUsername";
                            await botClient.EditMessageText(
                                chatId: callbackQuery.Message!.Chat.Id,
                                messageId: callbackQuery.Message.MessageId,
                                text: $"Siz Premium (Gift) tanladingiz. Bu sovg'a orqali rasmiylashtiriladi va log-in shart emas.\n\nIltimos, uni qabul qilib oluvchining Telegram ID/Username kiriting (masalan, @username):",
                                replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "cancel_buy")),
                                cancellationToken: cancellationToken
                            );
                        }
                        else
                        {
                            user.State = "AwaitingPremiumPhone";
                            await botClient.EditMessageText(
                                chatId: callbackQuery.Message!.Chat.Id,
                                messageId: callbackQuery.Message.MessageId,
                                text: $"Siz Premium (Login) tanladingiz.\n\nIltimos, nomeringizni kiriting (+998xx...):",
                                replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "cancel_buy")),
                                cancellationToken: cancellationToken
                            );
                        }
                    }

                    user.TempData = JsonSerializer.Serialize(session);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else if (data == "confirm_data")
        {
            var session = string.IsNullOrEmpty(user.TempData) ? null : JsonSerializer.Deserialize<OrderSessionData>(user.TempData);
            if (session != null)
            {
                var product = await dbContext.Products.FindAsync(new object[] { session.ProductId }, cancellationToken);
                if (product != null)
                {
                    user.State = "AwaitingPayment";
                    await dbContext.SaveChangesAsync(cancellationToken);

                    decimal finalPrice = session.CustomPriceUZS > 0 ? session.CustomPriceUZS : product.PriceUZS;
                    string finalName = session.ProductName ?? product.Name;

                    string paymentMsg = $"Siz {finalName} ni tanladingiz.\n" +
                                        $"To'lov summasi: {finalPrice:N0} UZS\n\n" +
                                        $"📍 To'lovni amalga oshirishda ehtiyot bo'ling, bizga aynan ko'rsatilgan miqdor yetib kelishi kerak.\n\n" +
                                        $"⚠️ MUHIM: Agar to'lovni ATM (bankomat) orqali amalga oshirsangiz, komissiya uchun +1,000 so'm qo'shib yuboring.\n\n" +
                                        $"💳 Karta: 5440 8103 0946 4823 (Uzcard/Mastercard)\n" +
                                        $"Qabul qiluvchi: Uchqunov.M\n\n" +
                                        $"To'lov qilingach, chekni (skrinshot) shu yerga rasm ko'rinishida yuboring.";

                    var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "cancel_buy")
                    );

                    await botClient.EditMessageText(
                        chatId: callbackQuery.Message!.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: paymentMsg,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
        else if (data == "has2fa_yes" || data == "has2fa_no")
        {
            var session = string.IsNullOrEmpty(user.TempData) ? null : JsonSerializer.Deserialize<OrderSessionData>(user.TempData);
            if (session != null && user.State == "AwaitingRoblox2FAQuery")
            {
                session.Roblox2FA = data == "has2fa_yes" ? "Ha (Admin bog'lanadi)" : "Yo'q";
                user.State = "AwaitingDataConfirmation";
                user.TempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Optionally remove the inline keyboard from the message they just clicked
                await botClient.EditMessageReplyMarkup(
                    chatId: callbackQuery.Message!.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    replyMarkup: null,
                    cancellationToken: cancellationToken
                );

                await ShowDataConfirmationAsync(botClient, callbackQuery.Message.Chat.Id, session, cancellationToken);
            }
        }
        else if (data.StartsWith("finish_order_"))
        {
            if (int.TryParse(data.Split('_')[2], out int orderId))
            {
                var order = await dbContext.Orders
                    .Include(o => o.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

                if (order != null && order.Status == "Approved")
                {
                    order.Status = "Completed";
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Update the button message in the group
                    await botClient.EditMessageText(
                        chatId: callbackQuery.Message!.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: callbackQuery.Message.Text + "\n\n✅ Buyurtma bajarildi!",
                        cancellationToken: cancellationToken
                    );

                    // Switch user state to collect review
                    order.User.State = $"AwaitingReview_{order.Id}";
                    await dbContext.SaveChangesAsync(cancellationToken);

                    await botClient.SendMessage(
                        chatId: order.UserId,
                        text: $"Sizning {order.Product.Name} xaridingiz yakunlandi! Iltimos, admin / xizmat haqida fikringizni yozing (masalan: @wunego halol). Fikringiz jamoat guruhida e'lon qilinadi.",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
        else if (data.StartsWith("approve_") || data.StartsWith("reject_"))
        {
            bool isApprove = data.StartsWith("approve_");
            if (int.TryParse(data.Split('_')[1], out int orderId))
            {
                var order = await dbContext.Orders
                    .Include(o => o.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

                if (order != null && order.Status == "PendingConfirmation")
                {
                    order.Status = isApprove ? "Approved" : "Rejected";
                    // Only keep AdditionalInfo if approved so it can be forwarded,
                    // Actually, let's scrub it right after we send it to group.
                    
                    await botClient.EditMessageCaption(
                        chatId: callbackQuery.Message!.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        caption: callbackQuery.Message.Caption + $"\n\n{(isApprove ? "✅ Tasdiqlandi (Ma'lumotlar tizimdan doimiy o'chirildi)" : "❌ Rad etildi")}",
                        cancellationToken: cancellationToken
                    );

                    string userMsg = isApprove
                        ? $@"Sizning xaridingiz tasdiqlandi! Buyurtma qabul qilindi ✅"
                        : $@"Sizning xaridingiz rad etildi ❌. Iltimos, admin bilan bog'laning.";

                    await botClient.SendMessage(
                        chatId: order.UserId,
                        text: userMsg,
                        cancellationToken: cancellationToken
                    );

                    if (isApprove)
                    {
                        var config = _serviceProvider.GetRequiredService<IConfiguration>();
                        string? ordersGroupIdStr = config["OrdersGroupId"];
                        string? ordersTopicIdStr = config["OrdersTopicId"];

                        if (long.TryParse(ordersGroupIdStr, out long groupId) && int.TryParse(ordersTopicIdStr, out int topicId))
                        {
                            // Parse the product name to show a clean display
                            string displayName = order.Product.Name;
                            if (displayName == "Telegram Stars (Maxsus)")
                                displayName = "Telegram Stars";

                            // Price: for Stars it's stored in AdditionalInfo already, for others use product price
                            string priceDisplay = order.Product.Category == "TelegramStars"
                                ? "" // Already in additionalInfo
                                : $"\n💰 <b>To'lov:</b> {order.Product.PriceUZS:N0} UZS";

                            string internalOrderMsg = $"📊 <b>YANGI BUYURTMA</b>\n" +
                                                      new string('─', 20) + "\n\n" +
                                                      $"👤 <b>Mijoz:</b> @{order.User.Username}\n" +
                                                      $"🛍 <b>Mahsulot:</b> {displayName}" +
                                                      priceDisplay + "\n\n" +
                                                      $"📝 <b>Ma'lumotlar:</b>\n{order.AdditionalInfo}";

                            var fulfillmentKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Buyurtma Bajarildi", $"finish_order_{order.Id}")
                            );

                            await botClient.SendMessage(
                                chatId: groupId,
                                messageThreadId: topicId,
                                text: internalOrderMsg,
                                parseMode: ParseMode.Html,
                                replyMarkup: fulfillmentKeyboard,
                                cancellationToken: cancellationToken
                            );
                        }
                    }
                    
                    // Always scrub Additional info now that it's processed/forwarded!
                    order.AdditionalInfo = "SCRUBBED FOR SECURITY";
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram API Error");
        return Task.CompletedTask;
    }
}
