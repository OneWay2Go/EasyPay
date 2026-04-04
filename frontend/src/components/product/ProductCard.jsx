import React from 'react'
import { Star, ShieldCheck, ShoppingBag } from 'lucide-react'
import { useNavigate } from 'react-router-dom'

const ProductCard = ({ product }) => {
  const navigate = useNavigate()

  // Format price to Uzbek so'm (UZS)
  const formatPrice = (price) => {
    return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ') + " so'm"
  }

  return (
    <div className="glass animate-fade-in" style={{ 
      borderRadius: '24px', 
      overflow: 'hidden',
      display: 'flex',
      flexDirection: 'column',
      transition: 'transform 0.2s ease',
      cursor: 'pointer',
      position: 'relative'
    }}
    onClick={() => navigate(`/product/${product.id}`)}
    onMouseDown={(e) => e.currentTarget.style.transform = 'scale(0.96)'}
    onMouseUp={(e) => e.currentTarget.style.transform = 'scale(1)'}
    >
      {/* Product Image Placeholder */}
      <div style={{ 
        height: '140px', 
        background: `linear-gradient(135deg, rgba(108, 99, 255, 0.2), rgba(255, 107, 107, 0.2))`,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        position: 'relative'
      }}>
        <span style={{ fontSize: '12px', color: 'var(--text-secondary)', fontWeight: '600' }}>PHOTO</span>
        
        {/* Category Badge */}
        <div style={{ 
          position: 'absolute',
          top: '12px',
          left: '12px',
          background: 'rgba(0,0,0,0.4)',
          backdropFilter: 'blur(4px)',
          padding: '4px 10px',
          borderRadius: '12px',
          fontSize: '10px',
          fontWeight: '700',
          letterSpacing: '0.5px',
          textTransform: 'uppercase'
        }}>
          {product.category}
        </div>
      </div>

      <div style={{ padding: '16px' }}>
        <h3 style={{ fontSize: '15px', fontWeight: '700', marginBottom: '4px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
          {product.title}
        </h3>
        
        <div style={{ display: 'flex', alignItems: 'center', gap: '4px', marginBottom: '12px' }}>
          <Star size={12} fill="var(--accent-gold)" color="var(--accent-gold)" />
          <span style={{ fontSize: '12px', color: 'var(--text-secondary)', fontWeight: '600' }}>{product.rating}</span>
          <span style={{ fontSize: '12px', color: 'rgba(255,255,255,0.2)' }}>•</span>
          <div style={{ display: 'flex', alignItems: 'center', gap: '2px' }}>
            <ShieldCheck size={12} color="var(--success)" />
            <span style={{ fontSize: '10px', color: 'var(--success)', fontWeight: '700' }}>VERIFIED</span>
          </div>
        </div>

        <div style={{ 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'flex-end'
        }}>
          <div style={{ display: 'flex', flexDirection: 'column' }}>
            <span style={{ fontSize: '10px', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Price</span>
            <span style={{ fontSize: '15px', fontWeight: '800', color: 'var(--text-primary)' }}>{formatPrice(product.price)}</span>
          </div>
          
          <div style={{ 
            background: 'var(--primary)', 
            padding: '8px', 
            borderRadius: '12px',
            boxShadow: '0 4px 12px rgba(108, 99, 255, 0.4)'
          }}>
            <ShoppingBag size={16} />
          </div>
        </div>
      </div>
    </div>
  )
}

export default ProductCard
