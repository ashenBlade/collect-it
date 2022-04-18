import { Injectable } from '@nestjs/common';
import {User} from "./users.model";
import {InjectModel} from "@nestjs/sequelize";
import {CreateUserDto} from "./dto/create-user.dto";

@Injectable()
export class UsersService {

    constructor(@InjectModel(User) private usersRepository: typeof User) { }

    async createUser(dto: CreateUserDto) {
        const user = await this.usersRepository.create(dto);
        return user;
    }

    async getAllUsers() {
        const users = await this.usersRepository.findAll();
        return users;
    }

    async getUserByEmail(email: string) {
        const user = await this.usersRepository.findOne({
            where: {
                email: email,
            },
            include: {
                all: true,
            }
        })
        return user;
    }

    async getUserByUsername(username: string) {
        const user = await this.usersRepository.findOne({
            where: {
                username: username,
            },
            include: {
                all: true,
            }
        })
        return user;
    }
}
