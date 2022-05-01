import {
  ArgumentMetadata,
  BadRequestException,
  PipeTransform,
} from '@nestjs/common';
const emailRegex =
  /^[a-zA-Z\d.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?(?:\.[a-zA-Z\d](?:[a-zA-Z\d-]{0,61}[a-zA-Z\d])?)*$/;

export class ParseEmailPipe implements PipeTransform {
  transform(value: any, metadata: ArgumentMetadata): any {
    if (typeof value !== 'string') {
      throw new BadRequestException({
        message: 'Email must be string type',
      });
    }

    if (!emailRegex.test(value)) {
      throw new BadRequestException({ message: 'Incorrect email' });
    }

    return value;
  }
}
