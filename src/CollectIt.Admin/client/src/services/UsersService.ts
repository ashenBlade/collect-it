import authorizedFetch from "./AuthorizedFetch";
import User from "../components/entities/user";

export class UsersService {
    private static readonly fetch = authorizedFetch();
    static async getUsersPagedAsync(pageNumber: number, pageSize: number): Promise<User[]> {
        const result = await UsersService.fetch('http://localhost:7000/api/v1/users', {
            body: JSON.stringify({
                pageNumber: pageNumber,
                pageSize: pageSize
            })
        });
        return await result.json();
    }
}