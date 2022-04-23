import {forwardRef, Module} from "@nestjs/common";
import {SequelizeModule} from "@nestjs/sequelize";
import {Resource} from "./resources.model";
import {ResourcesService} from "./resources.service";
import {MusicsController} from "./music/musics.controller";
import {AuthModule} from "../auth/auth.module";
import {MusicsModule} from "./music/musics.module";

@Module({
    providers: [ResourcesService],
    controllers: [],
    imports: [
        SequelizeModule.forFeature([Resource]),
        AuthModule,
        forwardRef(() => MusicsModule)
    ],
    exports: [
        ResourcesService
    ]
})
export class ResourcesModule { }