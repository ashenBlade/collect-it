import {HttpException, HttpStatus, Injectable} from '@nestjs/common';
import { UsersService } from "../users/users.service";
import { LoginDto } from "./dto/login.dto";
import { JwtService } from "@nestjs/jwt";
import { User } from "../users/users.model";
import * as bcrypt from 'bcrypt';
import * as identity from 'aspnetcore-identity-password-hasher';
import {STATUS_CODES} from "http";

@Injectable()
export class AuthService {
    constructor(private userService: UsersService,
                private jwtService: JwtService) { }

    async login({username, password}: LoginDto) {
        const user = await this.userService.getUserByUsername(username);
        if (!user) {
            throw new HttpException('Invalid Username/Password couple', HttpStatus.BAD_REQUEST);
        }
        if (!await identity.verify(password, user.passwordHash)) {
            throw new HttpException('Invalid Username/Password couple', HttpStatus.BAD_REQUEST);
        }
        return await this.generateToken(user);
    }

    async generateToken(user: User) {
        const payload = {
            id: user.id,
            email: user.email,
            roles: user.roles.map(role => role.name)
        };
        return {
            token: this.jwtService.sign(payload, {
                secret: process.env.JWT_PRIVATE_KEY,
                expiresIn: '24h',
                subject: user.id.toString(),
            })
        }
    }
}
