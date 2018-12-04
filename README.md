# MongoConnector.Net
MongoDB to Elasticsearch 

# Modules
    1. ConnectorApp
        - Init, Run, Exit 
    2. ConnectorIntermediary
        - MongoDB to ES broker role
    3. MongoOpLogManager
        - MongoDB Oplog Read Class
    4. Clients
        - MongoDBClient, NESTClient

# Usage
1. MongoDB 설치 및 Replica Set으로 구성.
    - https://docs.mongodb.com/manual/tutorial/deploy-replica-set/
    - Version 3.2 이상
        
2. ElasticSearch 설치 
    - https://www.elastic.co/guide/kr/elasticsearch/reference/current/gs-installation.html
    - Version 6.0 이상

3. dotnet build (command line)
4. dotnet run MongoConnector.Net.dll

# Resources
    - [yougov/mongo-connector](https://github.com/yougov/mongo-connector)

# License
This software is licensed under the [Apache 2 license](LICENSE.txt), quoted below.

Copyright 2015 Kakao Corp. <https://github.com/aikloy>

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this project except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
