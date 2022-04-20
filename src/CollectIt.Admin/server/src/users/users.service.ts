import {HttpException, HttpStatus, Injectable} from '@nestjs/common';
import {User} from "./users.model";
import {InjectModel} from "@nestjs/sequelize";
import {CreateUserDto} from "./dto/create-user.dto";
// import {Role} from "../roles/roles.model";
import {RolesService} from "../roles/roles.service";
import {Role} from "../roles/roles.model";

@Injectable()
export class UsersService {

    constructor(@InjectModel(User) private usersRepository: typeof User,
                private rolesService: RolesService) { }

    async createUser(dto: CreateUserDto) {
        const user = await this.usersRepository.create(dto);
        return user;
    }

    async getAllUsersAsync(pageNumber: number, pageSize: number) {
        const users = await this.usersRepository.findAndCountAll({
            limit: pageSize,
            offset: (pageNumber - 1) * pageSize,
            include: Role,
            order: [['Id', 'ASC']]
        });
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

    async addRoleToUser(userId: number, roleName: string) {
        const user = await this.usersRepository.findByPk(userId);
        const role = await this.rolesService.getRoleByName(roleName);
        if (!(role && user)) {
            throw new HttpException('No user or role found', HttpStatus.BAD_REQUEST);
        }
        await user.$add('role', role.id);
    }

    async removeRoleFromUser(userId: number, roleName: string) {
        try {
            const user = await this.usersRepository.findByPk(userId);
            const role = await this.rolesService.getRoleByName(roleName);
            await user.$remove('role', role.id);
        } catch (e) {
            console.log(e);
            throw new HttpException({
                message: 'Error while deleting role'
            }, HttpStatus.BAD_REQUEST);
        }
    }

    async getUserByIdAsync(userId: number) {
        const user = await this.usersRepository.findByPk(userId, {
            include: Role,
        });
        return user;
    }
}
