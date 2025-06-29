import useFetchPosts from "../hooks/PostHooks";

const PostList = () => {
   // const [posts, setPosts ] = useState<Post[]>([]);

    const { data } = useFetchPosts();

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
            <th>Comments</th>
            <th>Content</th>
            <th>Date Created</th>
          </tr>
        </thead>
        <tbody>
          {data && data.map((p) => (
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