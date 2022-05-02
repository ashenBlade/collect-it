import {
  BelongsTo,
  Column,
  DataType,
  ForeignKey,
  Model,
  Table,
} from 'sequelize-typescript';
import { RestrictionType } from './restriction-type';
import { User } from '../../users/users.model';

export class CreateRestrictionInterface {
  readonly restrictionType: RestrictionType;
  authorId: number | null;
  daysAfter: number | null;
  daysTo: number | null;
  sizeBytes: number | null;
  tags: string[] | null;
}

@Table({
  tableName: 'Restriction',
  timestamps: false,
  paranoid: false,
})
export class Restriction extends Model<
  Restriction,
  CreateRestrictionInterface
> {
  @Column({
    field: 'Id',
    type: DataType.INTEGER,
    primaryKey: true,
    autoIncrement: true,
    allowNull: false,
  })
  id: number;

  @Column({
    field: 'RestrictionType',
    type: DataType.INTEGER,
    allowNull: false,
  })
  restrictionType: RestrictionType;

  @Column({
    field: 'AuthorId',
    type: DataType.INTEGER,
    allowNull: true,
    defaultValue: null,
  })
  @ForeignKey(() => User)
  authorId: number | null;

  @BelongsTo(() => User)
  author: User;

  @Column({
    field: 'DaysAfter',
    type: DataType.INTEGER,
    allowNull: true,
    validate: {
      min: 1,
    },
    defaultValue: null,
  })
  daysAfter: number | null;

  @Column({
    field: 'DaysTo',
    type: DataType.INTEGER,
    allowNull: true,
    validate: {
      min: 1,
    },
    defaultValue: null,
  })
  daysTo: number | null;

  @Column({
    field: 'SizeBytes',
    type: DataType.INTEGER,
    allowNull: true,
    defaultValue: null,
  })
  sizeBytes: number | null;

  @Column({
    field: 'Tags',
    type: DataType.ARRAY(DataType.STRING),
    allowNull: true,
    defaultValue: null,
  })
  tags: string[] | null;
}
