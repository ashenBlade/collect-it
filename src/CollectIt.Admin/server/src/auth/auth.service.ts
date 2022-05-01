import { Injectable } from '@nestjs/common';
import { UsersService } from '../users/users.service';
import { JwtService } from '@nestjs/jwt';
import { User } from '../users/users.model';
import * as identity from 'aspnetcore-identity-password-hasher';

@Injectable()
export class AuthService {
  constructor(
    private userService: UsersService,
    private jwtService: JwtService,
  ) {}

  async login(username: string, password: string) {
    const user = await this.userService.getUserByUsernameAsync(username);
    if (!(user && (await identity.verify(password, user.passwordHash)))) {
      throw new Error('Invalid Username/Password couple');
    }
    return await this.generateToken(user);
  }

  async generateToken(user: User) {
    const payload = {
      id: user.id,
      email: user.email,
      roles: user.roles.map((role) => role.name),
    };
    return this.jwtService.sign(payload, {
      secret: process.env.JWT_PRIVATE_KEY,
      expiresIn: '120d',
      subject: user.id.toString(),
    });
  }

  verifyJwt(jwt: string) {
    return this.jwtService.verify(jwt, {
      secret: process.env.JWT_PRIVATE_KEY,
    });
  }
}
