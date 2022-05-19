import authorizedFetch from "./AuthorizedFetch";
import NotFoundError from "../utils/NotFoundError";
import { serverAddress } from "../constants";
import User from "../components/entities/user";
import { Role } from "../components/entities/role";

const baseApiPath = `${serverAddress}/api/v1/users`;

export class UsersService {
    private static readonly fetch = authorizedFetch();
    static async getUsersPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number}) {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await UsersService.fetch(`${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get users from server');
        }
        const json = await response.json();
        const users: User[] = json.users;
        const totalCount = Number(json.totalCount);
        return {
            totalCount: totalCount,
            users: users
        };
    }

    static async findUserByIdAsync(id: number) {
        if (!Number.isInteger(id)) {
            throw new Error(`Id must be integer. Given id: ${id}`);
        }
        const result = await UsersService.fetch(`${baseApiPath}/${id}`, {
            method: 'GET'
        });

        if (!result.ok) {
            if (result.status === 404) throw new NotFoundError()
            throw new Error((await result.json()).message);
        }

        const user: User = await result.json();
        return user;
    }

    static async findUserByUsernameAsync(username: string) {
        if (!username) throw new Error('Username is not provided');
        const response = await UsersService.fetch(`${baseApiPath}/with-username/${encodeURIComponent(username)}`, {
            method: 'GET'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            throw new Error((await response.json()).message);
        }

        const user: User = await response.json();
        return user;
    }

    static async findUserByEmailAsync(email: string) {
        if (!email) throw new Error('Email not provided');
        const response = await UsersService.fetch(`${baseApiPath}/with-email/${encodeURIComponent(email)}`, {
            method: 'GET'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            throw new Error((await response.json()).message);
        }

        const user: User = await response.json();
        return user;
    }

    static async changeUsernameAsync(id: number, username: string) {
        if (!username) throw new Error('Username not provided');
        if (username.length < 6) throw new Error('Username length must be greater than 6');
        if (!Number.isInteger(id)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${id}/username`, {
            method: 'POST',
            body: JSON.stringify({
                username
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) throw new Error((await response.json()).message);
    }

    static async searchUsersByUsernameEntry(usernameEntry: string) {
        if(!usernameEntry) throw new Error('Entry can not be empty');
        const response = await UsersService.fetch(`${baseApiPath}/search/with-username/${encodeURIComponent(usernameEntry)}`, {
            method: 'GET'
        })
        const users: User[] = await response.json();
        return users;
    }

    static async changeEmailAsync(id: number, email: string) {
        if (!email) throw new Error('Email not provided');
        if (email.length < 6) throw new Error('Email length must be greater than 6');
        if (!Number.isInteger(id)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${id}/email`, {
            method: 'POST',
            body: JSON.stringify({
                email
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            console.error(`Invalid email change`);
            console.error(response);
            throw new Error((await response.json()).message)
        }
    }

    static async addUserToRoleAsync(id: number, role: Role) {
        if (!Number.isInteger(id)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${id}/roles`, {
            method: 'POST',
            body: JSON.stringify({
                role: role
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            const json = await response.json();
            throw new Error(json.message);
        }
    }

    static async removeUserFromRoleAsync(id: number, role: Role) {
        if (!Number.isInteger(id)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${id}/roles`, {
            method: 'DELETE',
            body: JSON.stringify({
                role: role
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            const json = await response.json();
            throw new Error(json.message);
        }
    }

    static async lockoutUserAsync(userId: number) {
        if (!Number.isInteger(userId)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${userId}/deactivate`, {
            method: 'POST',
        });
        if (!response.ok) {
            const json = await response.json();
            throw new Error(json.message);
        }
    }

    static async activateUserAsync(userId: number) {
        if (!Number.isInteger(userId)) throw new Error('Id must be integer');
        const response = await UsersService.fetch(`${baseApiPath}/${userId}/activate`, {
            method: 'POST',
        });
        if (!response.ok) {
            const json = await response.json();
            throw new Error(json.message);
        }
    }

    static async updateUserBatchAsync({ name, email, id }: { name: string; email: string; id: number }) {
        if (!(name && email)) throw new Error('Name or email not provided');
        if (name.length < 6) throw new Error('Min name length is 6');
        if (!Number.isInteger(id)) throw new Error('Id must be integer');
        const emailRegex =
            /^[a-zA-Z\d.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?(?:\.[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?)*$/;
        if (!emailRegex.test(email)) throw new Error('Email is not valid');
        const res = await UsersService.fetch(`${baseApiPath}/${id}`, {
            method: 'PATCH',
            body: JSON.stringify({
                name, email
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        })
        if (!res.ok) {
            if (res.status === 404) throw new NotFoundError();
            const err = await res.json();
            console.error(err)
            throw new Error('Could not update user')
        }
    }
}