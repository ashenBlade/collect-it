import {
  BelongsTo,
  Column,
  DataType,
  Model,
  Table,
} from 'sequelize-typescript';
import { Resource } from '../resources.model';

export interface CreateImageInterface {
  readonly id: number;
}

@Table({
  tableName: 'Images',
  timestamps: false,
  paranoid: false,
})
export class Image extends Model<Image, CreateImageInterface> {
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
}
