import {
  BadRequestException,
  Body,
  Controller,
  Get,
  HttpCode,
  NotFoundException,
  Param,
  ParseIntPipe,
  Patch,
  Post,
  Query,
  UsePipes,
  ValidationPipe,
} from '@nestjs/common';
import { SubscriptionsService } from './subscriptions.service';
import { Authorize } from '../auth/jwt-auth.guard';
import { AuthorizeAdmin } from '../auth/admin-jwt-auth.guard';
import { CreateSubscriptionDto } from './dto/create-subscription.dto';
import CreationException from '../common/creation.exception';
import { ToReadSubscriptionDto } from './dto/read-subscription.dto';
import { UpdateSubscriptionDto } from "./dto/update-subscription.dto";
import { NotFoundError } from "rxjs";

@Authorize()
@Controller('api/v1/subscriptions')
export class SubscriptionsController {
  constructor(private subscriptionsService: SubscriptionsService) {}

  @Get(':subscriptionId')
  @UsePipes(ValidationPipe)
  async getSubscriptionById(
    @Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
  ) {
    const subscription =
      await this.subscriptionsService.getSubscriptionByIdAsync(subscriptionId);
    if (!subscription) {
      throw new NotFoundException({
        message: `Subscription with id = ${subscriptionId} not found`,
      });
    }
    return ToReadSubscriptionDto(subscription);
  }

  @Post('')
  @AuthorizeAdmin()
  // @UsePipes(ValidationPipe)
  async createSubscription(@Body() dto: CreateSubscriptionDto) {
    // console.log(JSON.stringify(dto))
    // throw new NotFoundException();
    try {
      const subscription =
        await this.subscriptionsService.createSubscriptionAsync(
          dto.name,
          dto.description,
          dto.price,
          dto.monthDuration,
          dto.resourceType,
          dto.maxResourcesCount,
          false,
          dto.restriction?.restrictionType,
          dto.restriction?.authorId,
          dto.restriction?.sizeBytes,
          dto.restriction?.daysTo,
          dto.restriction?.daysAfter,
          dto.restriction?.tags,
        );
      console.log(
        `Created subscription: ${JSON.stringify(subscription, null, 2)}`,
      );
      return subscription;
    } catch (e) {
      if (e instanceof CreationException) {
        console.error(`Creation exception`)
        console.error(e)
        throw new BadRequestException({
          message: e.message,
          errors: e.errors,
        });
      }
      console.error(e)
      throw new BadRequestException(
        {
          message: 'Could not create subscription. Try later.',
          errors: [],
        },
        'Something went wrong on server',
      );
    }
  }

  @Get('')
  async getSubscriptionsList(
    @Query('page_number', new ParseIntPipe()) pageNumber: number,
    @Query('page_size', new ParseIntPipe()) pageSize: number
  ) {
    try {
      const paged =
        await this.subscriptionsService.getSubscriptionsPagedAsync(
          pageNumber,
          pageSize,
        );
      return {
        totalCount: paged.count,
        subscriptions: paged.rows.map((s) => ToReadSubscriptionDto(s)),
      };
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: 'Something went wrong on server. Try later.',
      });
    }
  }

  @Post(':subscriptionId/name')
  @UsePipes(ValidationPipe)
  @AuthorizeAdmin()
  async changeSubscriptionName(
    @Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
    @Body('name') name: string,
  ) {
    try {
      await this.subscriptionsService.changeSubscriptionNameAsync(
        subscriptionId,
        name,
      );
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Post(':subscriptionId/description')
  @AuthorizeAdmin()
  async changeSubscriptionDescription(
    @Body('description') description: string,
    @Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
  ) {
    try {
      await this.subscriptionsService.changeSubscriptionDescriptionAsync(
        subscriptionId,
        description,
      );
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Post(':subscriptionId/activate')
  @AuthorizeAdmin()
  async activateSubscription(
    @Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
  ) {
    try {
      await this.subscriptionsService.activateSubscriptionAsync(subscriptionId);
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Post(':subscriptionId/deactivate')
  @AuthorizeAdmin()
  async deactivateSubscription(
    @Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
  ) {
    try {
      await this.subscriptionsService.deactivateSubscriptionAsync(
        subscriptionId,
      );
    } catch (e) {
      console.error(e);
      throw new BadRequestException({
        message: e.message,
      });
    }
  }

  @Patch(':subscriptionId')
  @AuthorizeAdmin()
  @HttpCode(204)
  // @UsePipes(ValidationPipe)
  async updateSubscription(@Param('subscriptionId', new ParseIntPipe()) subscriptionId: number,
      @Body() dto: UpdateSubscriptionDto) {
      try {
        await this.subscriptionsService.updateSubscription(subscriptionId, dto.name, dto.description);
      } catch (e) {
        if (e instanceof NotFoundError) {
          throw new NotFoundException();
        }
        console.error(e)
        throw new BadRequestException({
          message: 'Fuck this shit'
        })
      }
  }
}
