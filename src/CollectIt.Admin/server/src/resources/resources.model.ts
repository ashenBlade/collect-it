import {
  BelongsTo,
  Column,
  DataType,
  Model,
  Table,
} from 'sequelize-typescript';
import { User } from '../users/users.model';

export interface CreateResourceInterface {
  readonly name: string;
  readonly ownerId: number;
  readonly uploadDate: Date;
  readonly tags: string[];
  readonly extension: string;
}

@Table({
  tableName: 'Resources',
  timestamps: false,
  paranoid: false,
})
export class Resource extends Model<Resource, CreateResourceInterface> {
  @Column({
    type: DataType.INTEGER,
    primaryKey: true,
    field: 'Id',
    unique: true,
    allowNull: false,
    autoIncrement: true,
    autoIncrementIdentity: true,
  })
  readonly id: number;

  @Column({
    type: DataType.TEXT,
    field: 'FileName',
    allowNull: false,
  })
  readonly filename: string;

  @Column({
    type: DataType.TEXT,
    field: 'Name',
    allowNull: false,
  })
  readonly name: string;

  @Column({
    type: DataType.DATE,
    field: 'UploadDate',
  })
  readonly uploadDate: Date;

  @Column({
    type: DataType.ARRAY(DataType.TEXT),
    field: 'Tags',
    defaultValue: [],
    allowNull: false,
  })
  readonly tags: string[];

  @Column({
    type: DataType.TEXT,
    field: 'Extension',
    allowNull: false,
  })
  readonly extension: string;

  @Column({
    type: DataType.INTEGER,
    field: 'OwnerId',
    allowNull: false,
  })
  readonly ownerId: number;

  @BelongsTo(() => User, 'OwnerId')
  readonly owner: User;
}
