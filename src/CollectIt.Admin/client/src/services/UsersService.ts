import authorizedFetch from "./AuthorizedFetch";

export class UsersService {
    private static readonly fetch = authorizedFetch();
    static async getUsersPagedAsync(pageNumber: number, pageSize: number) {
        const result = await UsersService.fetch(`http://localhost:7000/api/v1/users?page_number=${pageNumber}&page_size=${pageSize}`, {});
        let json = await result.json();
        return {
            totalCount: json.totalCount,
            users: json.users
        };
    }
}