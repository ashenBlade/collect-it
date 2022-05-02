import {
  BelongsTo,
  Column,
  DataType,
  Model,
  Table,
} from 'sequelize-typescript';
import { Resource } from '../resources.model';

export interface CreateVideoInterface {
  readonly id: number;
  readonly duration: number;
}

@Table({
  tableName: 'Videos',
  timestamps: false,
  paranoid: false,
})
export class Video extends Model<Video, CreateVideoInterface> {
  @Column({
    autoIncrementIdentity: false,
    autoIncrement: false,
    field: 'Id',
    unique: true,
    primaryKey: true,
    type: DataType.INTEGER,
    allowNull: false,
  })
  readonly id: number;

  @BelongsTo(() => Resource, 'Id')
  readonly resource: Resource;

  @Column({
    field: 'Duration',
    unique: false,
    type: DataType.INTEGER,
    allowNull: false,
  })
  readonly duration: number;
}
