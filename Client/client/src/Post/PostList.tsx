import { useState, useEffect } from "react"
import { Post } from "../Types/post";
import config from "../config";

const PostList = () => {
    const [posts, setPosts ] = useState<Post[]>([]);



/// Problem: fetchPosts() is inside the component → runs on every render → triggers setPosts() → re-renders → loop.
/// Solution: Move it into useEffect so it only runs once on initial mount.
    useEffect(() => {
        const fetchPosts = async () => {
            const rsp = await fetch(`${config.BaseApiUrl}/api/posts`);
            const data = await rsp.json();
            setPosts(data);
        };

        fetchPosts();
    }, []); //

  return (
    <div>
      <div className="row mb-2">
        <h5 className="themeFontColor text-center">
          Houses currently on the market
        </h5>
      </div>
      <table className="table table-hover">
        <thead>
          <tr>
            <th>Address</th>
            <th>Country</th>
            <th>Asking Price</th>
          </tr>
        </thead>
        <tbody>
          {posts.map((p) => (
            <tr key={p.id}>
                <td>{p.comments}</td>
                <td>{p.content}</td>
                <td>{p.LastModified}</td>
                <td>{p.dateCreate}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};



export default PostList;