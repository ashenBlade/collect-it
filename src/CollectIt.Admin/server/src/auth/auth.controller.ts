import { BadRequestException, Body, Controller, Post } from '@nestjs/common';
import { UsersService } from '../users/users.service';
import { LoginDto } from './dto/login.dto';
import { AuthService } from './auth.service';

@Controller('auth')
export class AuthController {
  constructor(
    private userService: UsersService,
    private authService: AuthService,
  ) {}
  @Post('login')
  async login(@Body() { password, username }: LoginDto) {
    let jwt: string | null;
    if (!(password && username)) {
      throw new BadRequestException({
        message: 'Username or password not provided',
      });
    }
    try {
      jwt = await this.authService.login(username, password);
    } catch (e) {
      throw new BadRequestException({
        message: e.message,
      });
    }
    return {
      token: jwt,
    };
  }
}
