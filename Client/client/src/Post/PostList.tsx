import { useState } from "react"
import { Post } from "../Types/post";
import config from "../config";

const PostList = () => {
    const [posts, setPosts ] = useState<Post[]>([]);
    

    const fetchPosts = async () => {
        const rsp = await fetch(`${config.BaseApiUrl}/api/posts`); 
        const posts = await rsp.json();
        setPosts(posts);
    }

    fetchPosts(); 

  return (
    <div>
      <div className="row mb-2">
        <h5 className="themeFontColor text-center">
          Dummy Posts
        </h5>
      </div>
      <table className="table table-hover">
        <thead>
          <tr>
            <th>Comments</th>
            <th>Content</th>
          </tr>
        </thead>
        <tbody>
          {posts.map((p) => (
            <tr key={p.id}>
                <td>{p.comments}</td>
                <td>{p.content}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PostList;