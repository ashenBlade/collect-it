import { Injectable } from '@nestjs/common';

@Injectable()
export class UsersService {
  private readonly _users: any[] = [{id: 1}, {id: 2}]
  async getUsersAsync() {
    return this._users;
  }

  async getUserById(userId: number) {
    return this._users.find(u => u.id === userId);
  }
}
