import React, { useState } from 'react'
import { Search, ShoppingBag, Gamepad2, Star, Sparkles } from 'lucide-react'
import ProductCard from '../components/product/ProductCard'

const MOCK_PRODUCTS = [
  { id: '1', title: '1,000 Robux', price: 125000, category: 'Robux', seller: 'Uchqunov.M', rating: 4.9, stock: 99 },
  { id: '2', title: '5,000 Robux', price: 600000, category: 'Robux', seller: 'PremiumStore', rating: 4.8, stock: 42 },
  { id: '3', title: '100 Telegram Stars', price: 24000, category: 'Stars', seller: 'EasyPay Official', rating: 5.0, stock: 1000 },
  { id: '4', title: 'Telegram Premium (1 mo)', price: 45000, category: 'Premium', seller: 'GiftBot', rating: 4.7, stock: 50 },
  { id: '5', title: 'Fortnite V-Bucks (2.8k)', price: 320000, category: 'Games', seller: 'GamerStore', rating: 4.9, stock: 8 },
]

const CATEGORIES = [
  { id: 'all', name: 'All', icon: <ShoppingBag size={18} /> },
  { id: 'Robux', name: 'Robux', icon: <Gamepad2 size={18} /> },
  { id: 'Stars', name: 'Stars', icon: <Star size={18} /> },
  { id: 'Premium', name: 'Premium', icon: <Sparkles size={18} /> },
]

const HomePage = () => {
  const [activeCategory, setActiveCategory] = useState('all')
  const [searchQuery, setSearchQuery] = useState('')

  const filteredProducts = MOCK_PRODUCTS.filter(p => {
    const matchesCategory = activeCategory === 'all' || p.category === activeCategory
    const matchesSearch = p.title.toLowerCase().includes(searchQuery.toLowerCase())
    return matchesCategory && matchesSearch
  })

  return (
    <div className="home-page animate-fade-in" style={{ padding: '20px', paddingBottom: '90px' }}>
      <header style={{ marginBottom: '24px' }}>
        <h1 style={{ fontSize: '28px', fontWeight: '800', marginBottom: '8px' }}>Explore Market</h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '15px' }}>Find the best deals in so'm</p>
      </header>

      {/* Search Bar */}
      <div className="glass" style={{ 
        display: 'flex', 
        alignItems: 'center', 
        padding: '12px 16px', 
        borderRadius: '16px',
        marginBottom: '24px',
        gap: '12px'
      }}>
        <Search size={20} color="var(--text-secondary)" />
        <input 
          type="text" 
          placeholder="Search products..." 
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{ 
            background: 'none', 
            border: 'none', 
            color: 'white', 
            width: '100%',
            fontSize: '16px',
            outline: 'none'
          }}
        />
      </div>

      {/* Category List */}
      <div className="categories" style={{ display: 'flex', gap: '10px', overflowX: 'auto', marginBottom: '24px', paddingBottom: '5px' }}>
        {CATEGORIES.map(cat => (
          <button 
            key={cat.id}
            onClick={() => setActiveCategory(cat.id)}
            className="glass" 
            style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '8px',
              padding: '10px 16px', 
              borderRadius: '30px',
              whiteSpace: 'nowrap',
              backgroundColor: activeCategory === cat.id ? 'var(--primary)' : 'var(--bg-card)',
              transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
              transform: activeCategory === cat.id ? 'scale(1.05)' : 'scale(1)',
              border: activeCategory === cat.id ? '1px solid var(--primary)' : '1px solid var(--glass-border)'
            }}
          >
            {cat.icon}
            <span style={{ fontWeight: '600', fontSize: '14px' }}>{cat.name}</span>
          </button>
        ))}
      </div>

      {/* Product Grid */}
      <div className="product-grid" style={{ 
        display: 'grid', 
        gridTemplateColumns: 'repeat(2, 1fr)', 
        gap: '16px' 
      }}>
        {filteredProducts.map(product => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
      
      {filteredProducts.length === 0 && (
        <div style={{ textAlign: 'center', padding: '40px 20px', color: 'var(--text-secondary)' }}>
          <p>No products found for "{searchQuery}"</p>
        </div>
      )}
    </div>
  )
}

export default HomePage
