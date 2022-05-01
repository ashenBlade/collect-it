import {
  BelongsTo,
  Column,
  DataType,
  Table,
  Model,
} from 'sequelize-typescript';
import { Resource } from '../resources.model';

export interface CreateMusicInterface {
  readonly duration: number;
  readonly id: number;
}

@Table({
  tableName: 'Musics',
  timestamps: false,
  paranoid: false,
})
export class Music extends Model<Music, CreateMusicInterface> {
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
