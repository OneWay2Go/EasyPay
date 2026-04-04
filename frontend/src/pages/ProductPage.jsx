import React, { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { ChevronLeft, ShoppingCart, Minus, Plus, Star, ShieldCheck, MessageSquare } from 'lucide-react'

// Mock Data (will be replaced by API)
const MOCK_PRODUCTS = [
  { id: '1', title: '1,000 Robux', price: 125000, category: 'Robux', seller: 'Uchqunov.M', rating: 4.9, stock: 99, description: 'Sizning hisobingizga 1,000 Robux o\'tkazib beriladi. Xarid uchun login va parol talab etiladi.' },
  { id: '2', title: '5,000 Robux', price: 600000, category: 'Robux', seller: 'PremiumStore', rating: 4.8, stock: 42, description: 'VIP paket: 5,000 Robux. Tezkor yetkazish 15-30 daqiqa ichida.' },
]

const ProductPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const [quantity, setQuantity] = useState(1)
  const [product, setProduct] = useState(null)

  useEffect(() => {
    // Find product in mock data
    const found = MOCK_PRODUCTS.find(p => p.id === id) || MOCK_PRODUCTS[0]
    setProduct(found)
  }, [id])

  if (!product) return null

  const formatPrice = (price) => {
    return (price * quantity).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ') + " so'm"
  }

  return (
    <div className="product-page animate-fade-in" style={{ padding: '0 0 100px 0' }}>
      {/* Header Image */}
      <div style={{ 
        height: '300px', 
        background: `linear-gradient(180deg, rgba(0,0,0,0.2) 0%, rgba(13,14,30,1) 100%), linear-gradient(135deg, #6C63FF, #FF6B6B)`,
        position: 'relative',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}>
        {/* Back Button */}
        <button 
          onClick={() => navigate(-1)}
          className="glass" 
          style={{ 
            position: 'absolute', top: '20px', left: '20px', 
            padding: '10px', borderRadius: '50%', color: 'white',
            display: 'flex', alignItems: 'center', justifyContent: 'center'
          }}
        >
          <ChevronLeft size={24} />
        </button>
        
        <h1 style={{ fontSize: '100px', opacity: 0.1, fontWeight: '900' }}>GAME</h1>
      </div>

      <div style={{ padding: '24px', marginTop: '-40px', position: 'relative' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '16px' }}>
          <div>
            <span style={{ fontSize: '12px', fontWeight: '800', color: 'var(--primary)', textTransform: 'uppercase', letterSpacing: '1px' }}>
              {product.category}
            </span>
            <h1 style={{ fontSize: '28px', fontWeight: '800', marginBottom: '8px' }}>{product.title}</h1>
          </div>
          <div className="glass" style={{ padding: '8px 12px', borderRadius: '16px', display: 'flex', alignItems: 'center', gap: '6px' }}>
            <Star size={18} fill="var(--accent-gold)" color="var(--accent-gold)" />
            <span style={{ fontWeight: '700' }}>{product.rating}</span>
          </div>
        </div>

        {/* Seller Info Card */}
        <div className="glass" style={{ 
          padding: '16px', 
          borderRadius: '20px', 
          marginBottom: '24px',
          display: 'flex',
          alignItems: 'center',
          gap: '16px'
        }}>
          <div style={{ 
            width: '48px', 
            height: '48px', 
            borderRadius: '50%', 
            background: 'var(--primary)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '18px',
            fontWeight: '800'
          }}>
            {product.seller[0]}
          </div>
          <div style={{ flex: 1 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ fontWeight: '700' }}>{product.seller}</span>
              <ShieldCheck size={16} color="var(--success)" />
            </div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>Verified Seller • 1.2k deals</span>
          </div>
          <button style={{ color: 'var(--primary)', padding: '8px' }}>
            <MessageSquare size={20} />
          </button>
        </div>

        <p style={{ color: 'var(--text-secondary)', lineHeight: '1.6', marginBottom: '32px' }}>
          {product.description}
        </p>

        {/* Quantity and Price */}
        <div style={{ 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'space-between',
          marginBottom: '40px'
        }}>
          <div style={{ display: 'flex', flexDirection: 'column' }}>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)', marginBottom: '4px' }}>Total Price</span>
            <span style={{ fontSize: '24px', fontWeight: '800', color: 'var(--text-primary)' }}>{formatPrice(product.price)}</span>
          </div>

          <div className="glass" style={{ 
            display: 'flex', 
            alignItems: 'center', 
            gap: '16px', 
            padding: '8px 16px', 
            borderRadius: '30px' 
          }}>
            <button 
              onClick={() => setQuantity(Math.max(1, quantity - 1))}
              style={{ color: 'white', opacity: quantity === 1 ? 0.3 : 1 }}
            >
              <Minus size={20} />
            </button>
            <span style={{ fontSize: '18px', fontWeight: '800', minWidth: '24px', textAlign: 'center' }}>{quantity}</span>
            <button 
              onClick={() => setQuantity(Math.min(product.stock, quantity + 1))}
              style={{ color: 'white' }}
            >
              <Plus size={20} />
            </button>
          </div>
        </div>

        {/* Buy Button */}
        <button 
          onClick={() => navigate('/checkout')}
          style={{ 
            width: '100%', 
            background: 'var(--primary)', 
            padding: '18px', 
            borderRadius: '20px',
            fontSize: '18px',
            fontWeight: '800',
            color: 'white',
            boxShadow: '0 10px 25px rgba(108, 99, 255, 0.4)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            gap: '12px'
          }}
        >
          <ShoppingCart size={22} />
          Buy Now
        </button>
      </div>
    </div>
  )
}

export default ProductPage
