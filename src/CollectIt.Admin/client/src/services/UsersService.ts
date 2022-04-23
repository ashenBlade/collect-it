import useAuthFetch from "./AuthorizedFetch";
import User from "../components/entities/user";

export class UsersService {
    async getUsersPagedAsync(pageNumber: number, pageSize: number): Promise<User[]> {
        const auth = useAuthFetch();
        const result = await auth('http://localhost:7000/api/v1/users', {
            body: JSON.stringify({
                pageNumber: pageNumber,
                pageSize: pageSize
            })
        });
        return await result.json();
    }
}