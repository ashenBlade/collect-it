import {NestFactory} from '@nestjs/core';
import {AppModule} from './app.module';

const PORT = process.env.PORT || 7000;

async function bootstrap() {
    const app = await NestFactory.create(AppModule, {cors: true});
    const origin = ['http://localhost:3000']; // for testing local
    const client = process.env.CLIENT_ADDRESS;
    if (client) origin.push(client);
    app.enableCors({
        origin: origin,
        allowedHeaders: '*',
    });
    await app.listen(PORT, () => {
        console.log(`Server started on port ${PORT}`);
    });
}
bootstrap();
