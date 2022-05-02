import { Column, ForeignKey, Model, Table } from 'sequelize-typescript';
import { User } from '../users/users.model';
import { Role } from './roles.model';

@Table({
  tableName: 'AspNetUserRoles',
  timestamps: false,
  paranoid: false,
})
export class UserRole extends Model<UserRole> {
  @Column({
    allowNull: false,
    field: 'UserId',
  })
  @ForeignKey(() => User)
  userId: number;

  @Column({
    allowNull: false,
    field: 'RoleId',
  })
  @ForeignKey(() => Role)
  roleId: number;
}
