import { Injectable } from '@nestjs/common';
import { UsersService } from "../users/users.service";
import { LoginDto } from "./dto/login.dto";
import { JwtService } from "@nestjs/jwt";
import { User } from "../users/users.model";
import * as bcrypt from 'bcrypt';

@Injectable()
export class AuthService {
    constructor(private userService: UsersService,
                private jwtService: JwtService) { }

    async login({username, password}: LoginDto) {
        const user = await this.userService.getUserByUsername(username);
        if (!user) {
            throw new Error('Invalid Username/Password couple');
        }
        const hashedPassword = await bcrypt.hash(password, Number(process.env.PASSWORD_SALT));
        if (user.passwordHash !== hashedPassword) {
            throw new Error('Invalid Username/Password couple');
        }
        return await this.generateToken(user);
    }

    async generateToken(user: User) {
        const payload = {
            id: user.id,
            email: user.email,
            roles: user.roles
        };
        return {
            token: this.jwtService.sign(payload)
        }
    }
}
