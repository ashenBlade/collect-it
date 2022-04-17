import {Controller, Get, Param} from '@nestjs/common';
import { UsersService } from './users.service';

@Controller('/api/v1/users')
export class UsersController {
  constructor(private readonly usersService: UsersService) {}

  @Get('')
  getHello() {
    return this.usersService.getUsersAsync();
  }

  @Get(':userId')
  getUserId(@Param('userId') userId: number) {
    return this.usersService.getUserById(userId);
  }
}
