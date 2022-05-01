import { Injectable } from '@nestjs/common';
import { InjectModel } from '@nestjs/sequelize';
import { Role } from './roles.model';

@Injectable()
export class RolesService {
  constructor(@InjectModel(Role) private roleRepository: typeof Role) {}

  async getAllRoles() {
    return await this.roleRepository.findAll();
  }

  async getRoleById(roleId: number) {
    return await this.roleRepository.findOne({
      where: {
        id: roleId,
      },
    });
  }

  async getRoleByName(roleName: string) {
    return await this.roleRepository.findOne({
      where: {
        normalizedName: roleName.toUpperCase(),
      },
    });
  }

  async getRolesForUser(userId: number) {
    throw Error('Can get roles for user yet');
  }
}
