import {HttpException, HttpStatus, Injectable, NotFoundException} from '@nestjs/common';
import {User} from "./users.model";
import {InjectModel} from "@nestjs/sequelize";
import {CreateUserDto} from "./dto/create-user.dto";
// import {Role} from "../roles/roles.model";
import {RolesService} from "../roles/roles.service";
import {Role} from "../roles/roles.model";

const emailRegex = /^[a-zA-Z\d.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?(?:\.[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?)*$/;

@Injectable()
export class UsersService {

    constructor(@InjectModel(User) private usersRepository: typeof User,
                private rolesService: RolesService) { }

    async createUser(dto: CreateUserDto) {
        return await this.usersRepository.create(dto);
    }

    async getAllUsersAsync(pageNumber: number, pageSize: number) {
        return await this.usersRepository.findAndCountAll({
            limit: pageSize,
            offset: (pageNumber - 1) * pageSize,
            include: Role,
            order: [['Id', 'ASC']]
        });
    }

    async getUserByEmailAsync(email: string) {
        return await this.usersRepository.findOne({
            where: {
                email: email,
            },
            include: Role
        });
    }

    async getUserByUsernameAsync(username: string) {
        return await this.usersRepository.findOne({
            where: {
                username: username,
            },
            include: [{
                all: true,
            }]
        });
    }

    async addRoleToUser(userId: number, roleName: string) {
        const user = await this.usersRepository.findByPk(userId);
        const role = await this.rolesService.getRoleByName(roleName);
        if (!(role && user)) {
            throw new NotFoundException('No user or role found');
        }
        await user.$add('role', role.id);
        await user.save();
    }

    async removeRoleFromUser(userId: number, roleName: string) {
        try {
            const user = await this.usersRepository.findByPk(userId);
            const role = await this.rolesService.getRoleByName(roleName);
            await user.$remove('role', role.id);
            await user.save();
        } catch (e) {
            console.log(e);
            throw new HttpException({
                message: 'Error while deleting role'
            }, HttpStatus.BAD_REQUEST);
        }
    }

    async getUserByIdAsync(userId: number) {
        return await this.usersRepository.findByPk(userId, {
            include: [{all: true}],
        });
    }

    async changeEmailAsync(userId: number, email: string) {
        if (!emailRegex.test(email)) {
            throw new Error('Email is not in correct form');
        }
        await this.usersRepository.update({email: email},
            {
                where: {
                    id: userId
                }
            });
    }
}
