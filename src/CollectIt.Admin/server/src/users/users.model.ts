import {Column, DataType, Model, Table} from "sequelize-typescript";

export interface UsersModelInterface {
    email: string;
    username: string;
    passwordHash: string;
}

@Table({
    tableName: 'AspNetUsers',
})
export class User extends Model<User, UsersModelInterface> {
    @Column({
        allowNull: false,
        type: DataType.INTEGER,
        unique: true,
        autoIncrement: true,
        primaryKey: true,
        field: 'Id'
    })
    id: number;

    @Column({
        allowNull: true,
        type: DataType.CHAR,
        unique: false,
        field: 'UserName'
    })
    username: string;

    @Column({
        allowNull: true,
        type: DataType.CHAR,
        unique: false,
        field: 'Email'
    })
    email: string;

    @Column({
        allowNull: true,
        type: DataType.TEXT,
        unique: false,
        field: 'PasswordHash',
    })
    passwordHash: string;
}