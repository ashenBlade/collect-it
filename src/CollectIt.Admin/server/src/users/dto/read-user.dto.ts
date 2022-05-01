import { User } from '../users.model';

export interface ReadUserDto {
  readonly id: number;
  readonly email: string;
  readonly username: string;
  readonly roles: string[];
}

export const ToReadUserDto = (user: User): ReadUserDto => ({
  id: user.id,
  email: user.email,
  username: user.username,
  roles: user.roles.map((r) => r.name),
});
