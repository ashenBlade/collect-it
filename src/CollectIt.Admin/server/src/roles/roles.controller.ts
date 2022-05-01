import { Controller, Get, Param } from '@nestjs/common';
import { RolesService } from './roles.service';
import { ReadRoleDto } from './dto/read-role.dto';

@Controller('api/v1/roles')
export class RolesController {
  constructor(private roleService: RolesService) {}

  @Get('')
  async getAllRoles() {
    return await this.roleService.getAllRoles();
  }

  @Get(':roleId')
  async getRoleById(@Param('roleId') roleId: number): Promise<ReadRoleDto> {
    const role = await this.roleService.getRoleById(roleId);
    return {
      id: role.id,
      name: role.name,
    };
  }
}
