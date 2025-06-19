import { useState } from 'react'
import './App.css'
import PostList from '../Post/PostList'
import Header from './Header'

function App() {
  const [count, setCount] = useState(0)

  return (
      <div className='container'>
        <Header subtitle="This is where your header data goes" />
        <PostList />
      </div>
  )
}

export default App
