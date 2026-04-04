using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EasyPay.Core.Entities;
using EasyPay.Core.Enums;
using EasyPay.Infrastructure.Database;
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
        var dbContext = scope.ServiceProvider.GetRequiredService<EPContext>();

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

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, EPContext dbContext, CancellationToken cancellationToken)
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
    
    private async Task HandleGroupMessageAsync(ITelegramBotClient botClient, Message message, EPContext dbContext, CancellationToken cancellationToken)
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
                text: $"Assalomu alaykum, {message.From?.FirstName}! 👋\nEasyPay orqali Robux va boshqa to'lovlarni amalga oshiring:",
                replyParameters: message.MessageId,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task HandlePrivateMessageAsync(ITelegramBotClient botClient, Message message, EPContext dbContext, CancellationToken cancellationToken)
    {
        // Unified User Retrieval: Check by TelegramId
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == message.From!.Id, cancellationToken);
        
        if (user == null)
        {
            // If doesn't exist by TelegramId, check by Username (optional, let's just create new)
            user = new User
            {
                UserId = Guid.NewGuid(),
                TelegramId = message.From!.Id,
                Username = message.From.Username ?? $"user_{message.From.Id}",
                PasswordHash = "TG_USER", // Dummy, they use TWA for real auth
                Email = "tg@easypay.uz",
                BotState = "Idle",
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // --- REVIEW COLLECTION HANDLING ---
        if (user.BotState.StartsWith("AwaitingReview_"))
        {
            if (string.IsNullOrEmpty(message.Text)) return;
            
            var orderIdStr = user.BotState.Split('_')[1];
            if (!Guid.TryParse(orderIdStr, out Guid orderId)) return;

            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);
            if (order != null)
            {
                var config = _serviceProvider.GetRequiredService<IConfiguration>();
                string? reviewGroupIdStr = config["ReviewsGroupId"];
                string? reviewTopicIdStr = config["ReviewsTopicId"];

                if (long.TryParse(reviewGroupIdStr, out long groupId) && int.TryParse(reviewTopicIdStr, out int topicId))
                {
                    string productName = order.ProductTitleSnapshot;
                    
                    string reviewMsg = $"🌟 <b>Yangi Fikr</b> 🌟\n\n" +
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
                
                user.BotState = "Idle";
                await dbContext.SaveChangesAsync(cancellationToken);
                
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Fikringiz uchun rahmat! ✅",
                    cancellationToken: cancellationToken
                );
            }
            return;
        }

        if (message.Text?.StartsWith("/start") == true)
        {
            user.BotState = "Idle";
            user.BotTempData = null;
            await dbContext.SaveChangesAsync(cancellationToken);
            await ShowMainMenuAsync(botClient, message.Chat.Id, cancellationToken);
            return;
        }

        // --- BOT PIPELINE LOGIC (Unified Entities) ---
        if (!string.IsNullOrEmpty(message.Text))
        {
            var text = message.Text;
            var session = string.IsNullOrEmpty(user.BotTempData) ? new OrderSessionData() : JsonSerializer.Deserialize<OrderSessionData>(user.BotTempData);

            if (user.BotState == "AwaitingStarsAmount")
            {
                if (int.TryParse(text, out int stars) && stars >= 50 && stars <= 10000)
                {
                    decimal price = stars * 240m;
                    session!.CustomPriceUZS = price;
                    session.ProductName = $"{stars} Telegram Stars";
                    
                    user.BotState = "AwaitingTelegramUsername";
                    user.BotTempData = JsonSerializer.Serialize(session);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Yaxshi! Endi Stars yuboriladigan Telegram Username ni kiriting (masalan: @username):",
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendMessage(chatId: message.Chat.Id, text: "❌ 50 dan 10.000 gacha raqam kiriting:", cancellationToken: cancellationToken);
                }
                return;
            }
            
            if (user.BotState == "AwaitingTelegramUsername")
            {
                session!.TargetTelegramUsername = text;
                user.BotState = "AwaitingDataConfirmation";
                user.BotTempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await ShowDataConfirmationAsync(botClient, message.Chat.Id, session, cancellationToken);
                return;
            }

            if (user.BotState == "AwaitingPremiumPhone")
            {
                session!.PhoneNumber = text;
                user.BotState = "AwaitingDataConfirmation";
                user.BotTempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await ShowDataConfirmationAsync(botClient, message.Chat.Id, session, cancellationToken);
                return;
            }

            if (user.BotState == "AwaitingRobloxUsername")
            {
                session!.RobloxUsername = text;
                user.BotState = "AwaitingRobloxPassword";
                user.BotTempData = JsonSerializer.Serialize(session);
                await dbContext.SaveChangesAsync(cancellationToken);
                await botClient.SendMessage(chatId: message.Chat.Id, text: "Endi Roblox parolingizni kiriting:", cancellationToken: cancellationToken);
                return;
            }

            if (user.BotState == "AwaitingRobloxPassword")
            {
                user.BotState = "AwaitingRoblox2FAQuery"; 
                user.BotTempData = JsonSerializer.Serialize(session); // Simplified for this draft, add encryption in production
                await dbContext.SaveChangesAsync(cancellationToken);
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Ha", "has2fa_yes") },
                    new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Yo'q", "has2fa_no") }
                });

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Robloxda 2FA ulanganmi?",
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        // --- PAYMENT PHOTO HANDLING (Order Creation) ---
        if (message.Photo != null && message.Photo.Length > 0 && user.BotState == "AwaitingPayment")
        {
            var session = string.IsNullOrEmpty(user.BotTempData) ? null : JsonSerializer.Deserialize<OrderSessionData>(user.BotTempData);
            if (session == null || !session.ProductId.HasValue) return;

            var product = await dbContext.Products.FindAsync(new object[] { session.ProductId.Value }, cancellationToken);
            if (product == null) return;

            var photoFileId = message.Photo.Last().FileId;

            // Create Unified Order
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                ProductId = product.ProductId,
                BuyerId = user.UserId, // Current user is the buyer
                SellerId = product.SellerId, // Linked to product seller
                ProductTitleSnapshot = session.ProductName ?? product.Title,
                PriceAtPurchase = session.CustomPriceUZS > 0 ? session.CustomPriceUZS : product.CurrentPrice,
                Quantity = 1,
                Status = OrderStatus.InEscrow, // Moving directly to InEscrow or a custom "PendingAdmin" status
                ReceiptFileId = photoFileId,
                AdditionalInfo = session.RobloxUsername != null ? $"Roblox: {session.RobloxUsername}" : session.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Orders.Add(order);
            user.BotState = "Idle";
            user.BotTempData = null;
            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "✅ To'lov cheki qabul qilindi. Admin tomonidan tasdiqlanishini kuting.",
                cancellationToken: cancellationToken
            );

            // Notify admin logic...
        }
    }

    private async Task ShowMainMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Robux", "category_Robux") },
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Stars", "category_Stars") }
        });

        await botClient.SendMessage(
            chatId: chatId,
            text: "Xush kelibsiz! Kerakli bo'limni tanlang:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task ShowDataConfirmationAsync(ITelegramBotClient botClient, long chatId, OrderSessionData session, CancellationToken cancellationToken)
    {
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Tasdiqlash", "confirm_data") },
            new [] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", "cancel_buy") }
        });

        await botClient.SendMessage(
            chatId: chatId,
            text: $"Ma'lumotlar to'g'rimi?\nMahsulot: {session.ProductName}",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, EPContext dbContext, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == callbackQuery.From.Id, cancellationToken);
        if (user == null || callbackQuery.Data == null) return;

        var data = callbackQuery.Data;

        if (data.StartsWith("category_"))
        {
            string category = data.Split('_')[1];
            // Fetch products from main DB
            var products = await dbContext.Products.Where(p => p.Description.Contains(category)).ToListAsync();
            
            var buttons = products.Select(p => new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton> 
            { 
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"{p.Title} - {p.CurrentPrice:N0} so'm", $"startbuy_{p.ProductId}") 
            }).ToList();

            await botClient.EditMessageText(
                chatId: callbackQuery.Message!.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                text: $"{category} paketini tanlang:",
                replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                cancellationToken: cancellationToken
            );
        }
        else if (data.StartsWith("startbuy_"))
        {
            if (Guid.TryParse(data.Split('_')[1], out Guid productId))
            {
                var product = await dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
                if (product != null)
                {
                    user.BotState = "AwaitingRobloxUsername"; // Categorize logic based on product
                    user.BotTempData = JsonSerializer.Serialize(new OrderSessionData { ProductId = product.ProductId, ProductName = product.Title });
                    await dbContext.SaveChangesAsync(cancellationToken);
                    
                    await botClient.SendMessage(chatId: callbackQuery.Message!.Chat.Id, text: "Roblox Username kiriting:", cancellationToken: cancellationToken);
                }
            }
        }
        else if (data == "confirm_data")
        {
            user.BotState = "AwaitingPayment";
            await dbContext.SaveChangesAsync(cancellationToken);
            await botClient.SendMessage(chatId: callbackQuery.Message!.Chat.Id, text: "Karta: 8600... To'lov skrinshotini yuboring.", cancellationToken: cancellationToken);
        }
    }
}
