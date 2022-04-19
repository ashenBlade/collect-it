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
        return await this.authService.login(dto);
    }
}
