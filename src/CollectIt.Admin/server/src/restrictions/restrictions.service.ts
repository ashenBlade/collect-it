import { Injectable } from '@nestjs/common';
import {InjectModel} from "@nestjs/sequelize";
import {Restriction} from "./restriction.model";

@Injectable()
export class RestrictionsService {
    constructor(@InjectModel(Restriction) private restrictionsRepository: typeof Restriction) {  }
    async createAuthorRestrictionAsync(authorId: number) {


    }
}
