import {CanActivate, ExecutionContext, Injectable, UnauthorizedException, UseGuards} from "@nestjs/common";
import {Observable} from "rxjs";
import {JwtService} from "@nestjs/jwt";
import {User} from "../users/users.model";

@Injectable()
export class AdminJwtAuthGuard implements CanActivate {

    constructor(private jwtService: JwtService) {  }

    canActivate(context: ExecutionContext): boolean | Promise<boolean> | Observable<boolean> {
        const req = context.switchToHttp().getRequest();
        try {
            const authHeader: string = req.headers.authorization;
            const [bearer, token] = authHeader.split(' ');
            if (!((bearer === 'Bearer') && token)) {
                throw new UnauthorizedException({
                    message: 'Authorization header is not provided'
                });
            }
            const user = this.jwtService.verify(token);
            if (user.roles.some(r => r === 'Admin')) {
                req.user = user;
                return true;
            }
            throw new UnauthorizedException({
                message: 'User not in "Admin" role'
            })
        } catch (e) {
            if (e instanceof UnauthorizedException) {
                throw e;
            }
            console.error(e)
            throw new UnauthorizedException()
        }
    }
}

export const AuthorizeAdmin = () => UseGuards(AdminJwtAuthGuard)