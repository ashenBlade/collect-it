import { Body, Controller, HttpException, HttpStatus, Post } from '@nestjs/common';
import { UsersService } from "../users/users.service";
import { LoginDto } from "./dto/login.dto";
import { AuthService } from "./auth.service";

@Controller('auth')
export class AuthController {
    constructor(private userService: UsersService,
                private authService: AuthService) {
    }
    @Post('login')
    async login(@Body() dto: LoginDto) {
        try {
            return await this.authService.login(dto);
        } catch (e) {
            throw new HttpException(e.message, HttpStatus.BAD_REQUEST);
        }
    }
}
