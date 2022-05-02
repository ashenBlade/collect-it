import {
  BelongsToMany,
  Column,
  DataType,
  Model,
  Table,
} from 'sequelize-typescript';
import { User } from '../users/users.model';
import { UserRole } from './user-role.model';

export interface RolesModelInterface {
  value: string;
}

@Table({
  tableName: 'AspNetRoles',
  paranoid: false,
  timestamps: false,
})
export class Role extends Model<Role, RolesModelInterface> {
  @Column({
    allowNull: false,
    type: DataType.INTEGER,
    unique: true,
    autoIncrement: true,
    primaryKey: true,
    field: 'Id',
  })
  id: number;

  @Column({
    allowNull: true,
    type: DataType.CHAR,
    field: 'Name',
  })
  name: string;

  @Column({
    allowNull: true,
    type: DataType.CHAR,
    field: 'NormalizedName',
    unique: true,
  })
  normalizedName: string;

  @Column({
    allowNull: true,
    type: DataType.TEXT,
    field: 'ConcurrencyStamp',
  })
  concurrencyStamp: string;

  @BelongsToMany(() => User, () => UserRole)
  users: User[];
}
