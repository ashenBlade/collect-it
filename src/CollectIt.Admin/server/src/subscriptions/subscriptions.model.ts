import {
  BelongsTo,
  Column,
  DataType,
  ForeignKey,
  Model,
  Table,
} from 'sequelize-typescript';
import { ResourceType } from '../common/resource-type';
import { Restriction } from './restrictions/restriction.model';

export interface CreateSubscriptionInterface {
  name: string;
  description: string;
  monthDuration: number;
  price: number;
  appliedResourceType: string;
  maxResourcesCount: number;
  restrictionId: number | null;
  active: boolean;
}

@Table({
  tableName: 'Subscriptions',
  timestamps: false,
  paranoid: false,
})
export class Subscription extends Model<
  Subscription,
  CreateSubscriptionInterface
> {
  @Column({
    field: 'Id',
    type: DataType.INTEGER,
    allowNull: false,
    primaryKey: true,
    autoIncrementIdentity: true,
    autoIncrement: true,
  })
  id: number;

  @Column({
    field: 'Name',
    allowNull: false,
    type: DataType.TEXT,
  })
  name: string;

  @Column({
    field: 'Description',
    allowNull: false,
    type: DataType.TEXT,
  })
  description: string;

  @Column({
    field: 'MonthDuration',
    type: DataType.INTEGER,
    validate: {
      isNumeric: true,
      min: 1,
    },
  })
  monthDuration: number;

  @Column({
    field: 'Price',
    type: DataType.INTEGER,
    validate: {
      isNumeric: true,
      min: 0,
    },
  })
  price: number;

  @Column({
    field: 'AppliedResourceType',
    type: DataType.TEXT,
    allowNull: false,
    values: ['Any', 'Image', 'Video', 'Music'],
  })
  appliedResourceType: ResourceType;

  @Column({
    field: 'MaxResourcesCount',
    type: DataType.INTEGER,
    validate: {
      min: 1,
    },
    allowNull: false,
  })
  maxResourcesCount: number;

  @Column({
    field: 'Active',
    type: DataType.BOOLEAN,
    allowNull: false,
    defaultValue: false,
  })
  active: boolean;

  @Column({
    field: 'RestrictionId',
    type: DataType.INTEGER,
    references: 'Restriction',
    unique: true,
  })
  @ForeignKey(() => Restriction)
  restrictionId: number | null;

  @BelongsTo(() => Restriction)
  restriction: Restriction | null;
}
