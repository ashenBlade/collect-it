import {
  CanActivate,
  ExecutionContext,
  Injectable,
  UnauthorizedException,
  UseGuards,
} from '@nestjs/common';
import { Observable } from 'rxjs';
import { JwtService } from '@nestjs/jwt';

@Injectable()
export class AdminJwtAuthGuard implements CanActivate {
  constructor(private jwtService: JwtService) {}

  canActivate(context: ExecutionContext): boolean | Promise<boolean> | Observable<boolean> {
    const req = context.switchToHttp().getRequest();
    try {
      const authHeader: string | undefined = req.headers.authorization;
      if (!authHeader) {
        throw new UnauthorizedException({
          message: 'Authorization header is not provided',
        });
      }
      const [bearer, token] = authHeader.split(' ');
      if (!(bearer === 'Bearer' && token)) {
        throw new UnauthorizedException({
          message: 'Authorization header is invalid',
        });
      }
      const user = this.jwtService.verify(token, {
        secret: process.env.JWT_PRIVATE_KEY,
      });
      if (user.roles.some((r) => r === 'Admin')) {
        req.user = user;
        return true;
      }
      throw new UnauthorizedException({
        message: 'User not in "Admin" role',
      });
    } catch (e) {
      if (e instanceof UnauthorizedException) {
        throw e;
      }
      console.error(e);
      throw new UnauthorizedException();
    }
  }
}

export const AuthorizeAdmin = () => UseGuards(AdminJwtAuthGuard);
