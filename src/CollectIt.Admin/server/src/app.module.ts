import { Module } from '@nestjs/common';
import { SequelizeModule } from '@nestjs/sequelize'
import { UsersModule } from './users/users.module';
import {UsersService} from "./users/users.service";
import {UsersController} from "./users/users.controller";
import {ConfigModule} from "@nestjs/config";

@Module({
  controllers: [UsersController],
  providers: [UsersService],
  imports: [
      ConfigModule.forRoot({
          envFilePath: '.env'
      }),
      SequelizeModule.forRoot({
          dialect: 'postgres',
          host: 'localhost',
          port: 5432,
          username: 'postgres',
          password: 'password',
          database: 'library',
          autoLoadModels: true,
          synchronize: true,
      }),
      UsersModule
  ],
})
export class AppModule {

}
