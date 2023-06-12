# Blazor App Template

### Setup

- Clone the repository
- Rename all occurences of "MyApp" to desired name in:
  - project names
  - folder names
  - find and replace all occurences of "MyApp" keyword in the solution
- From MyApp.UI.Common, remove conditional package references:
```
<Choose>
    <When Condition="'$(Configuration)' == 'Release'">
      <ItemGroup>
        <PackageReference Include="Corelibs.Blazor.UIComponents" Version="1.0.0" />
      </ItemGroup>
    </When>
    <When Condition="'$(Configuration)' == 'Debug'">
      <ItemGroup>
        <Reference Include="Corelibs.Blazor.UIComponents">
          <HintPath>..\..\..\Corelibs.Blazor.UIComponents\src\Corelibs.Blazor.UIComponents\bin\Debug\net7.0\Corelibs.Blazor.UIComponents.dll</HintPath>
        </Reference>
      </ItemGroup>
    </When>
  </Choose>
```
- Add only Corelibs.Blazor.UIComponents:
```
<ItemGroup>
    <PackageReference Include="Corelibs.Blazor.UIComponents" Version="1.0.0" />
</ItemGroup>
```
