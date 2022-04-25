import {ArgumentMetadata, Injectable, PipeTransform} from "@nestjs/common";
import {ValidationError} from "sequelize";
import {ResourceType} from "./resource-type";

@Injectable()
export class ParseResourceTypePipe implements PipeTransform {
    transform(value: any, metadata: ArgumentMetadata): ResourceType {
        if (value instanceof String) {
            switch (value.toLowerCase()) {
                case 'any':
                    return ResourceType.Any;
                case 'image':
                    return ResourceType.Image;
                case 'video':
                    return ResourceType.Video;
                case 'musics':
                    return ResourceType.Music;
                default:
                    throw new ValidationError(`Could not parse resource type: ${value}`, [])
            }
        } else if (value instanceof Number) {
            switch (value) {
                case 0:
                    return ResourceType.Any;
                case 1:
                    return ResourceType.Image;
                case 2:
                    return ResourceType.Video;
                case 3:
                    return ResourceType.Music;
                default:
                    throw new ValidationError(`Could not parse resource type: ${value}`, [])
            }

        }
        throw new ValidationError('Resource type is not in valid form', [])
    }

}