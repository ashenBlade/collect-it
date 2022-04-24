import {forwardRef, Module} from "@nestjs/common";
import {SequelizeModule} from "@nestjs/sequelize";
import {Resource} from "./resources.model";
import {ResourcesService} from "./resources.service";
import {AuthModule} from "../auth/auth.module";
import {MusicsModule} from "./musics/musics.module";
import {VideosModule} from "./videos/videos.module";

@Module({
    providers: [ResourcesService],
    controllers: [],
    imports: [
        SequelizeModule.forFeature([Resource]),
        AuthModule,
        forwardRef(() => MusicsModule),
        forwardRef(() => VideosModule)
    ],
    exports: [
        ResourcesService
    ]
})
export class ResourcesModule { }