import {
  BelongsToMany,
  Column,
  DataType,
  Model,
  Table,
} from 'sequelize-typescript';
import { Role } from 'src/roles/roles.model';
import { UserRole } from '../roles/user-role.model';

export interface UsersModelInterface {
  email: string;
  username: string;
  passwordHash: string;
}

@Table({
  tableName: 'AspNetUsers',
  // Remove 'createdAt', 'deletedAt' columns
  paranoid: false,
  timestamps: false,
})
export class User extends Model<User, UsersModelInterface> {
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
    unique: false,
    field: 'UserName',
  })
  username: string;

  @Column({
    allowNull: true,
    type: DataType.CHAR,
    unique: true,
    field: 'NormalizedUserName',
  })
  normalizedUsername: string;

  @Column({
    allowNull: true,
    type: DataType.CHAR,
    unique: false,
    field: 'Email',
  })
  email: string;

  @Column({
    allowNull: true,
    type: DataType.CHAR,
    unique: true,
    primaryKey: false,
    field: 'NormalizedEmail',
  })
  normalizedEmail: string;

  @Column({
    allowNull: false,
    type: DataType.BOOLEAN,
    unique: false,
    defaultValue: false,
    field: 'EmailConfirmed',
  })
  emailConfirmed: boolean;

  @Column({
    allowNull: true,
    type: DataType.TEXT,
    unique: false,
    field: 'PasswordHash',
  })
  passwordHash: string;

  @Column({
    allowNull: true,
    type: DataType.TEXT,
    field: 'SecurityStamp',
  })
  securityStamp: string;

  @Column({
    allowNull: true,
    type: DataType.TEXT,
    field: 'ConcurrencyStamp',
  })
  concurrencyStamp: string;

  @Column({
    allowNull: true,
    field: 'PhoneNumber',
    type: DataType.TEXT,
  })
  phoneNumber: string;

  @Column({
    allowNull: false,
    field: 'PhoneNumberConfirmed',
    type: DataType.BOOLEAN,
    defaultValue: false,
  })
  phoneNumberConfirmed: boolean;

  @Column({
    allowNull: false,
    field: 'TwoFactorEnabled',
    type: DataType.BOOLEAN,
    defaultValue: false,
  })
  twoFactorEnabled: string;

  @Column({
    allowNull: true,
    field: 'LockoutEnd',
    type: DataType.DATE,
  })
  lockoutEnd: Date;

  @Column({
    allowNull: false,
    type: DataType.BOOLEAN,
    field: 'LockoutEnabled',
    defaultValue: false,
  })
  lockoutEnable: boolean;

  @Column({
    allowNull: false,
    type: DataType.INTEGER,
    field: 'AccessFailedCount',
    defaultValue: 0,
  })
  accessFailedCount: number;

  @BelongsToMany(() => Role, () => UserRole)
  roles: Role[];
}
