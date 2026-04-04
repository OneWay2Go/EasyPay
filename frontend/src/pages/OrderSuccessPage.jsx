import React, { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { Send, CheckCircle2, Clock, MessageCircle, User, ShieldCheck } from 'lucide-react'

const OrderSuccessPage = () => {
  const navigate = useNavigate()
  const chatEndRef = useRef(null)
  
  // States to simulate order lifecycle
  const [status, setStatus] = useState('InEscrow') // InEscrow, Delivered, Completed
  const [isBuyer, setIsBuyer] = useState(true) // Simulating viewing as Buyer
  const [messages, setMessages] = useState([
    { id: 1, sender: 'System', text: 'Order placed successfully! Funds are held in escrow.', isSystem: true },
    { id: 2, sender: 'Seller', text: 'Hello! I am preparing your 1,000 Robux now. Please wait a few minutes.', isSystem: false },
  ])
  const [inputText, setInputText] = useState('')

  // Scroll to bottom on new message
  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages])

  const handleSendMessage = () => {
    if (!inputText.trim()) return
    const newMsg = {
      id: Date.now(),
      sender: isBuyer ? 'You' : 'Seller',
      text: inputText,
      isSystem: false
    }
    setMessages([...messages, newMsg])
    setInputText('')
  }

  const handleSellerDeliver = () => {
    setStatus('Delivered')
    setMessages([...messages, { 
      id: Date.now(), 
      sender: 'System', 
      text: 'Seller has marked the order as Delivered. Buyer, please confirm receipt.', 
      isSystem: true 
    }])
  }

  const handleBuyerConfirm = () => {
    setStatus('Completed')
    setMessages([...messages, { 
      id: Date.now(), 
      sender: 'System', 
      text: 'Order Completed! Funds will be released to the seller in 2 days.', 
      isSystem: true 
    }])
  }

  return (
    <div className="order-chat-page animate-fade-in" style={{ display: 'flex', flexDirection: 'column', height: '100vh', backgroundColor: 'var(--bg-dark)' }}>
      {/* Header */}
      <header className="glass" style={{ padding: '16px 20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center', zIndex: 10 }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '4px' }}>
            <span style={{ fontWeight: '800', fontSize: '18px' }}>Order #TR-8821</span>
            {status === 'InEscrow' && <Clock size={16} color="var(--accent-gold)" />}
            {status === 'Delivered' && <Clock size={16} color="var(--primary)" />}
            {status === 'Completed' && <CheckCircle2 size={16} color="var(--success)" />}
          </div>
          <span style={{ 
            fontSize: '12px', 
            fontWeight: '700', 
            textTransform: 'uppercase', 
            color: status === 'InEscrow' ? 'var(--accent-gold)' : (status === 'Delivered' ? '#6C63FF' : 'var(--success)') 
          }}>
            Status: {status.replace(/([A-Z])/g, ' $1').trim()}
          </span>
        </div>
        <button onClick={() => navigate('/')} style={{ color: 'var(--text-secondary)', fontSize: '14px', fontWeight: '700' }}>Close</button>
      </header>

      {/* Action Banner */}
      {status === 'Completed' && (
        <div style={{ 
          background: 'rgba(0, 200, 83, 0.1)', 
          padding: '12px 20px', 
          display: 'flex', 
          alignItems: 'center', 
          gap: '12px',
          borderBottom: '1px solid rgba(0, 200, 83, 0.1)'
        }}>
          <ShieldCheck size={20} color="var(--success)" />
          <span style={{ fontSize: '13px', color: 'var(--success)', fontWeight: '600' }}>
            Funds release scheduled for {new Date(Date.now() + 172800000).toLocaleDateString()}
          </span>
        </div>
      )}

      {/* Chat Area */}
      <div style={{ flex: 1, overflowY: 'auto', padding: '20px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
        {messages.map(msg => (
          msg.isSystem ? (
            <div key={msg.id} style={{ alignSelf: 'center', textAlign: 'center', padding: '10px 20px', background: 'rgba(255,255,255,0.05)', borderRadius: '12px', maxWidth: '80%' }}>
              <p style={{ fontSize: '11px', color: 'var(--text-secondary)', fontWeight: '700', letterSpacing: '0.5px' }}>{msg.text}</p>
            </div>
          ) : (
            <div key={msg.id} style={{ 
              alignSelf: msg.sender === 'You' ? 'flex-end' : 'flex-start',
              maxWidth: '80%',
              display: 'flex',
              flexDirection: 'column',
              alignItems: msg.sender === 'You' ? 'flex-end' : 'flex-start'
            }}>
              <div style={{ 
                padding: '12px 16px', 
                borderRadius: msg.sender === 'You' ? '20px 20px 4px 20px' : '20px 20px 20px 4px',
                background: msg.sender === 'You' ? 'var(--primary)' : 'var(--bg-card)',
                color: 'white',
                boxShadow: msg.sender === 'You' ? '0 4px 12px rgba(108, 99, 255, 0.2)' : 'none',
                border: msg.sender === 'You' ? 'none' : '1px solid var(--glass-border)'
              }}>
                <p style={{ fontSize: '15px', lineHeight: '1.4' }}>{msg.text}</p>
              </div>
              <span style={{ fontSize: '10px', color: 'var(--text-secondary)', marginTop: '4px', fontWeight: '600' }}>
                {msg.sender === 'You' ? 'You' : msg.sender} • {new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
              </span>
            </div>
          )
        ))}
        <div ref={chatEndRef} />
      </div>

      {/* Interaction Panel */}
      <div className="glass" style={{ padding: '20px', borderTop: '1px solid var(--glass-border)' }}>
        {/* Buyer Actions */}
        {isBuyer && status === 'Delivered' && (
          <button 
            onClick={handleBuyerConfirm}
            style={{ 
              width: '100%', background: 'var(--success)', color: 'white', padding: '16px', borderRadius: '16px', fontWeight: '800', marginBottom: '16px',
              display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '8px'
            }}>
            <CheckCircle2 size={20} />
            Confirm Receipt
          </button>
        )}

        {/* Seller Actions (Simulated) */}
        {!isBuyer && status === 'InEscrow' && (
          <button 
            onClick={handleSellerDeliver}
            style={{ 
              width: '100%', background: 'var(--primary)', color: 'white', padding: '16px', borderRadius: '16px', fontWeight: '800', marginBottom: '16px'
            }}>
            Mark as Delivered
          </button>
        )}

        {/* Input Area */}
        <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
          <div className="glass" style={{ flex: 1, padding: '12px 16px', borderRadius: '30px', display: 'flex', alignItems: 'center' }}>
            <input 
              type="text" 
              placeholder="Type message..." 
              value={inputText}
              onChange={(e) => setInputText(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
              style={{ background: 'none', border: 'none', color: 'white', outline: 'none', width: '100%', fontSize: '15px' }}
            />
          </div>
          <button 
            onClick={handleSendMessage}
            style={{ 
              width: '48px', height: '48px', borderRadius: '50%', background: 'var(--primary)', 
              display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'white'
            }}>
            <Send size={20} />
          </button>
        </div>
        
        {/* Toggle Role (For demo purposes) */}
        <div style={{ textAlign: 'center', marginTop: '12px' }}>
          <button onClick={() => setIsBuyer(!isBuyer)} style={{ fontSize: '10px', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '1px' }}>
            Switch to {isBuyer ? 'Seller' : 'Buyer'} View (Demo)
          </button>
        </div>
      </div>
    </div>
  )
}

export default OrderSuccessPage
