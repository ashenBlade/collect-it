import {
  ArgumentMetadata,
  BadRequestException,
  PipeTransform,
} from '@nestjs/common';

const usernameRegex = /^\w[\w\d]{5,}$/;

export class ParseUsernamePipe implements PipeTransform {
  transform(value: any, metadata: ArgumentMetadata): any {
    const str = value?.toString().trim();
    if (str && usernameRegex.test(value)) {
      return value;
    }
    throw new BadRequestException({
      success: false,
      errors: ['Username is incorrect'],
    });
  }
}
