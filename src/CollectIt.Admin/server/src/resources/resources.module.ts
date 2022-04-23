import {Module} from "@nestjs/common";
import {SequelizeModule} from "@nestjs/sequelize";
import {Resource} from "./resources.model";

@Module({
    imports: [
        SequelizeModule.forFeature([Resource])
    ]
})
export class ResourcesModule { }