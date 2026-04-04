import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ChevronLeft, Wallet, AlertCircle, CheckCircle2 } from 'lucide-react'

const CheckoutPage = () => {
  const navigate = useNavigate()
  const [isProcessing, setIsProcessing] = useState(false)
  const [walletBalance, setWalletBalance] = useState(250000) // Mock balance
  const [totalPrice, setTotalPrice] = useState(125000) // Mock total
  
  const handlePayment = () => {
    setIsProcessing(true)
    // Simulate API call
    setTimeout(() => {
      setIsProcessing(false)
      navigate('/order/success')
    }, 2000)
  }

  const hasEnoughFunds = walletBalance >= totalPrice

  return (
    <div className="checkout-page animate-fade-in" style={{ padding: '24px' }}>
      <header style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '32px' }}>
        <button onClick={() => navigate(-1)} style={{ color: 'white' }}><ChevronLeft size={24} /></button>
        <h1 style={{ fontSize: '24px', fontWeight: '800' }}>Confirm Payment</h1>
      </header>

      {/* Order Summary */}
      <div className="glass" style={{ padding: '24px', borderRadius: '24px', marginBottom: '24px' }}>
        <h2 style={{ fontSize: '18px', fontWeight: '800', marginBottom: '20px' }}>Order Summary</h2>
        
        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '12px' }}>
          <span style={{ color: 'var(--text-secondary)' }}>Item</span>
          <span style={{ fontWeight: '700' }}>1,000 Robux</span>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '12px' }}>
          <span style={{ color: 'var(--text-secondary)' }}>Quantity</span>
          <span style={{ fontWeight: '700' }}>1</span>
        </div>
        
        <div style={{ height: '1px', background: 'var(--glass-border)', margin: '16px 0' }}></div>
        
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span style={{ fontWeight: '700', fontSize: '18px' }}>Total Amount</span>
          <span style={{ fontWeight: '900', fontSize: '22px', color: 'var(--primary)' }}>
            {totalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ')} so'm
          </span>
        </div>
      </div>

      {/* Wallet Status */}
      <div className="glass" style={{ 
        padding: '20px', 
        borderRadius: '24px', 
        marginBottom: '40px',
        border: hasEnoughFunds ? '1px solid rgba(0, 200, 83, 0.2)' : '1px solid rgba(255, 82, 82, 0.2)',
        backgroundColor: hasEnoughFunds ? 'rgba(0, 200, 83, 0.02)' : 'rgba(255, 82, 82, 0.02)'
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '12px' }}>
          <Wallet size={20} color={hasEnoughFunds ? 'var(--success)' : 'var(--error)'} />
          <span style={{ fontWeight: '700', fontSize: '16px' }}>Your Wallet</span>
        </div>
        
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Current Balance</span>
          <span style={{ fontWeight: '800', fontSize: '18px' }}>
            {walletBalance.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ')} so'm
          </span>
        </div>

        {!hasEnoughFunds && (
          <div style={{ 
            marginTop: '16px', 
            padding: '12px', 
            background: 'rgba(255, 82, 82, 0.1)', 
            borderRadius: '12px',
            display: 'flex',
            gap: '10px',
            alignItems: 'center'
          }}>
            <AlertCircle size={18} color="var(--error)" />
            <span style={{ fontSize: '13px', color: 'var(--error)', fontWeight: '600' }}>Insufficient funds. Please top up your wallet via the bot.</span>
          </div>
        )}
      </div>

      {/* Pay Button */}
      <button 
        disabled={!hasEnoughFunds || isProcessing}
        onClick={handlePayment}
        style={{ 
          width: '100%', 
          background: hasEnoughFunds ? 'var(--primary)' : 'var(--text-secondary)', 
          padding: '20px', 
          borderRadius: '24px',
          fontSize: '18px',
          fontWeight: '800',
          color: 'white',
          boxShadow: hasEnoughFunds ? '0 10px 25px rgba(108, 99, 255, 0.4)' : 'none',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          gap: '12px',
          opacity: isProcessing ? 0.7 : 1,
          transition: 'all 0.3s ease'
        }}
      >
        {isProcessing ? (
          'Processing...'
        ) : (
          <>
            <CheckCircle2 size={24} />
            Pay Now
          </>
        )}
      </button>

      <p style={{ textAlign: 'center', color: 'var(--text-secondary)', fontSize: '12px', marginTop: '24px', lineHeight: '1.6' }}>
        By clicking Pay Now, you agree that your funds will be held in escrow until you confirm receiving the product.
      </p>
    </div>
  )
}

export default CheckoutPage
