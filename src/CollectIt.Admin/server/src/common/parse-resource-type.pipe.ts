import { ArgumentMetadata, Injectable, PipeTransform } from '@nestjs/common';
import { ValidationError } from 'sequelize';
import { ResourceType } from './resource-type';

@Injectable()
export class ParseResourceTypePipe implements PipeTransform {
  transform(value: any, metadata: ArgumentMetadata): ResourceType {
    if (value as ResourceType) return value;
    throw new ValidationError('Fuck', []);
  }
}
