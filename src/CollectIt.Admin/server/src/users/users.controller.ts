import {
  BadRequestException,
  Body,
  Controller,
  Delete,
  Get,
  HttpException,
  HttpStatus,
  NotFoundException,
  Param,
  ParseIntPipe,
  Patch,
  Post,
  Query,
  Res,
  UsePipes,
  ValidationPipe,
} from '@nestjs/common';
import { UsersService } from './users.service';
import { AuthorizeAdmin } from '../auth/admin-jwt-auth.guard';
import { Authorize } from '../auth/jwt-auth.guard';
import { ToReadUserDto } from './dto/read-user.dto';
import { Response } from 'express';
import { NotFoundError } from 'rxjs';
import { ParseEmailPipe } from '../common/parse-email.pipe';
import { ParseUsernamePipe } from '../common/parse-username.pipe';
import { UpdateUserBatchDto } from "./dto/update-user-batch.dto";

@Authorize()
@Controller('api/v1/users')
export class UsersController {
  constructor(private usersService: UsersService) {}

  @Get('')
  async getAll(
    @Query('page_number') pageNumber: number,
    @Query('page_size') pageSize: number,
  ) {
    const result = await this.usersService.getAllUsersAsync(
      pageNumber,
      pageSize,
    );
    const count = result.count;
    const users = result.rows;
    const dtos = users.map((u) => ({
      id: u.id,
      username: u.username,
      email: u.email,
      roles: u.roles?.map((r) => r.name) ?? [],
      lockout: u.lockoutEnable
    }));
    return {
      totalCount: count,
      users: dtos,
    };
  }

  @Get(':userId')
  async getUserById(
    @Param('userId', new ParseIntPipe()) userId: number) {
    const user = await this.usersService.getUserByIdAsync(userId);
    if (!user) throw new NotFoundException();
    return {
      id: user.id,
      username: user.username,
      email: user.email,
      roles: user?.roles.map(r => r.name) ?? [],
      lockout: user.lockoutEnable
    }
  }

  @Get('/with-email/:email')
  async getUserByEmail(@Param('email') email: string, @Res() res: Response) {
    const user = await this.usersService.getUserByEmailAsync(email);
    if (user) {
      res.send(ToReadUserDto(user));
    } else {
      res.status(HttpStatus.NOT_FOUND);
    }
    res.end();
  }

  @Get('/with-username/:username')
  async getUserByUsername(
    @Param('username', new ParseUsernamePipe()) username: string,
  ) {
    const user = await this.usersService.getUserByUsernameAsync(username);
    if (!user) {
      throw new NotFoundError(`User with username = ${username} not found`);
    }
    return ToReadUserDto(user);
  }

  @Post(':userId/email')
  @AuthorizeAdmin()
  async changeUserEmail(
    @Param('userId', new ParseIntPipe()) userId: number,
    @Body('email', new ParseEmailPipe()) email: string,
  ) {
    try {
      await this.usersService.changeEmailAsync(userId, email);
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Post(':userId/username')
  @AuthorizeAdmin()
  async changeUserUsername(
    @Param('userId', new ParseIntPipe()) userId: number,
    @Body('username', new ParseUsernamePipe()) username: string,
  ) {
    try {
      await this.usersService.changeUsernameAsync(userId, username);
    } catch (e) {
      if (e instanceof HttpException) {
        throw e;
      }
      console.error(e);
      throw new BadRequestException(
        {
          message: 'Something went wrong',
        },
        'Error occurred while changing username',
      );
    }
  }

  @Post(':userId/roles')
  @AuthorizeAdmin()
  async assignRole(
    @Param('userId', new ParseIntPipe()) userId: number,
    @Body('role') role: string,
  ) {
    await this.usersService.addRoleToUser(userId, role);
  }

  @Delete(':userId/roles')
  @AuthorizeAdmin()
  async removeRole(
    @Param('userId', new ParseIntPipe()) userId: number,
    @Body('role') role: string,
  ) {
    await this.usersService.removeRoleFromUser(userId, role);
  }

  @Post(':userId/activate')
  @AuthorizeAdmin()
  async activateUser(@Param('userId', new ParseIntPipe()) userId: number) {
    try {
      await this.usersService.activateUserAsync(userId);
    } catch (e) {
      if (e instanceof NotFoundError) throw new NotFoundException({
        message: 'User not found'
      });
      throw new BadRequestException();
    }
  }

  @Post(':userId/deactivate')
  @AuthorizeAdmin()
  async deactivateUser(@Param('userId', new ParseIntPipe()) userId: number) {
    try {
      await this.usersService.deactivateUserAsync(userId);
    } catch (e) {
      if (e instanceof NotFoundError) throw new NotFoundException({
        message: 'User not found'
      });
      throw new BadRequestException();
    }
  }

  @Get('search/with-username/:username')
  async searchUsersByUsernameEntry(@Param('username') username: string) {
    try {
      const users = await this.usersService.searchUsersByUsernameEntry(username);
      return users;
    } catch (e) {
      console.error(e)
      throw new BadRequestException();
    }
  }

  @Patch(':userId')
  @AuthorizeAdmin()
  @UsePipes(ValidationPipe)
  async updateUserBatch(@Param('userId', new ParseIntPipe())id: number, @Body()dto: UpdateUserBatchDto) {
    try {
      await this.usersService.updateUserBatch(id, dto.name, dto.email);
    } catch (e) {
      if (e instanceof NotFoundError) {
        throw new NotFoundException({
          message: 'User not found'
        })
      }
      console.error(e);
      throw new BadRequestException({
        message: 'Something went wrong'
      })
    }
  }
}
