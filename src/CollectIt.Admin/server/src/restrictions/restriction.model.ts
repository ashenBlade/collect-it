import {Column, DataType, Model, Table} from "sequelize-typescript";
import {RestrictionType} from "./restriction-type";

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
export class Restriction extends Model<Restriction> {
    @Column({
        field: 'Id',
        type: DataType.INTEGER,
        unique: true,
        autoIncrementIdentity: true,
    })
    id: number;

    @Column({
        field: 'RestrictionType',
        type: DataType.INTEGER,
        allowNull: false
    })
    restrictionType: RestrictionType;

    @Column({
        field: 'AuthorId',
        type: DataType.INTEGER,
        allowNull: true
    })
    authorId: number | null;

    @Column({
        field: 'DaysAfter',
        type: DataType.INTEGER,
        allowNull: true,
        validate: {
            min: 1
        }
    })
    daysAfter: number | null;

    @Column({
        field: 'DaysTo',
        type: DataType.INTEGER,
        allowNull: true,
        validate: {
            min: 1
        }
    })
    daysTo: number | null;

    @Column({
        field: 'SizeBytes',
        type: DataType.INTEGER,
        allowNull: true,
    })
    sizeBytes: number | null;

    @Column({
        field: 'Tags',
        type: DataType.ARRAY(DataType.STRING),
        allowNull: true
    })
    tags: string[] | null;
}