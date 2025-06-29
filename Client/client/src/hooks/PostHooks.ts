import axios, { AxiosError } from "axios";
import config from "../config";
import { useQuery } from "@tanstack/react-query";
import { Post } from "../Types/post";


const useFetchPosts = () => {
        return useQuery<Post[], AxiosError>({
            queryKey: ["posts"],
            queryFn: () =>
                axios.get(`${config.BaseApiUrl}/api/posts`).then((resp) => resp.data),
        });
    };

export default useFetchPosts;