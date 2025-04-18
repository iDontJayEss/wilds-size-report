local re = re
local sdk = sdk
local d2d = d2d
local imgui = imgui
local log = log
local json = json
local draw = draw
local thread = thread
local require = require
local tostring = tostring
local pairs = pairs
local ipairs = ipairs
local math = math
local string = string
local table = table
local type = type
local Core = require('_CatLib')
local CONST = require('_CatLib.const')
local Utils = require("_CatLib.utils")
local file = io.open('monster_probability.log', 'a')

local function WriteLine(line)
	file:write(line, "\n")
end

-- Code taken from https://www.nexusmods.com/monsterhunterwilds/mods/152 (Crown Only by lingsamuel)
----print(Utils["format_table_pretty"](myTable, nil))
local mod = Core.NewMod('MonsterSizeFileWriter')
----print("Hello Wilds~!")
--print("Hello Wilds~!")

--mod.HookFunc("app.EnemyManager", "create(app.EnemyDef.CONTEXT_SUB_CATEGORY, System.Int32, app.cContextCreateArg_Enemy, app.EnemyDef.SYNC_TYPE, via.GameObject)",
--function (args)
    --local CreateArg = Core.Cast(args[5])
    --local id = CreateArg:get_EmID()
    --local name = Core.GetEnemyName(id)
	--print(string.format("Creating enemy %s(%d) with size %d", name, id, CreateArg:get_ModelRandomSize()))
--end)

mod.HookFunc("app.EnemyManager", "create(System.Int32, app.cContextCreateArg_Enemy, app.EnemyDef.SYNC_TYPE)",
function (args)
    local CreateArg = Core.Cast(args[4])
    local id = CreateArg:get_EmID()
    local name = Core.GetEnemyName(id)
	WriteLine(string.format("Creating enemy %s(%d) with size %d", name, id, CreateArg:get_ModelRandomSize()))
	--print(string.format("Creating enemy %s(%d) with size %d", name, id, CreateArg:get_ModelRandomSize()))
end)

mod.HookFunc("app.EnemyUtil", "lotteryModelRandomSize_Boss(app.EnemyDef.ID, app.EnemyDef.LEGENDARY_ID, System.Guid)",
function (args)
    local id = sdk.to_int64(args[3])
    local name = Core.GetEnemyName(id)
    --print(string.format("Util Random: %s (%d)", name, id))
end, function (retval)
    return retval
end)

-- mod.HookFunc("app.user_data.EmParamRandomSize", "getRandomSizeTblData_Boss(app.EnemyDef.ID_Fixed, app.EnemyDef.LEGENDARY_ID, app.QuestDef.EM_REWARD_RANK, System.Int32)", function (args)
    
	-- local id = Core.FixedToEnum("app.EnemyDef.ID", sdk.to_int64(args[3]))
	-- if id == nil then
		-- --print(string.format("id nil for %s (%s)", args[3], type(args[3])))
	-- else
		-- local name = Core.GetEnemyName(id)
		-- --print(string.format("Random %s: %s", id, name))
	-- end
-- end)


local function PreHook(args)
	-- local id = Core.FixedToEnum("app.EnemyDef.ID", sdk.to_int64(args[3]))
	-- if id == nil then
		-- --print(string.format("PreHook|id nil for %s (%s)", args[3], type(args[3])))
	-- else
		-- local name = Core.GetEnemyName(id)
		-- --print(string.format("PreHook|getting statistics for %s: %s", id, name))
	-- end
	-- for i, arg in ipairs(args) do
	    -- ----print(Utils["format_table_pretty"](Core.Cast(arg), nil))
		-- --print(Utils["format_table_pretty"](arg, nil))
	-- end
    -- --print(Utils["format_table_pretty"](args, nil))
    local msg = string.format("Lottery Arg: %d", sdk.to_int64(args[3]))

    local this = Core.Cast(args[2])
	----print(type(this))
    local table = this._ProbDataTbl
	--for key, value in pairs(this) do
	    ----print(string.format("%s: %s", key, type(value)))
	--end
	----print(Utils["format_table_pretty"](this, nil))
	----print(Utils["format_table_pretty"](this._ProbDataTbl, nil))
    local biggest = 0
    local nonZeroProbBiggest = 0

    local smallest = 1000000
    local nonZeroProbSmallest = 1000000

    Core.ForEach(table, function (probData)
        local prob = probData._Prob
        local scale = probData._Scale
		WriteLine(string.format("  %s->%s",scale, prob))
		--print(string.format("probability: %s(%s) | scale: %s(%s)", prob, type(prob), scale, type(scale)))
        if scale > biggest then
            biggest = scale
        end
        if scale < smallest then
            smallest = scale
        end

        if prob > 0 then
            if scale > nonZeroProbBiggest then
                nonZeroProbBiggest = scale
            end
            if scale < nonZeroProbSmallest then
                nonZeroProbSmallest = scale
            end
        end
    end)

    if nonZeroProbBiggest > 0 then
        msg = msg .. string.format(", Big: %s -> %s", tostring(biggest), tostring(nonZeroProbBiggest))
        biggest = nonZeroProbBiggest
    end
    if nonZeroProbSmallest < 1000000 then
        msg = msg .. string.format(", Small: %s -> %s", tostring(smallest), tostring(nonZeroProbSmallest))
        smallest = nonZeroProbSmallest
    end

    -- local storage = thread.get_hook_storage()
    -- if mod.Config.BigCrown and mod.Config.SmallCrown then
        -- local ran = math.random()
        -- if ran > 0.5 then
            -- storage["retval"] = biggest
            -- msg = msg .. string.format("\nBig: %s (%0.2f)", tostring(storage["retval"]), ran)
        -- else
            -- storage["retval"] = smallest
            -- msg = msg .. string.format("\nSmall: %s (%0.2f)", tostring(storage["retval"]), ran)
        -- end
    -- elseif mod.Config.BigCrown then
        -- storage["retval"] = biggest
        -- msg = msg .. string.format("\nBig: %s", tostring(storage["retval"]))
    -- elseif mod.Config.SmallCrown then
        -- storage["retval"] = smallest
        -- msg = msg .. string.format("\nSmall: %s", tostring(storage["retval"]))
    -- end

    --print(msg)
end

local function PostHook(retval)
    -- local storage = thread.get_hook_storage()
    -- local result = storage["retval"]
    -- storage["retval"] = nil

    -- if result then
        -- --print("%s -> %s", tostring(sdk.to_int64(retval)), tostring(result))
        -- return sdk.to_ptr(result)
    -- end
	WriteLine(string.format("Lottery returned monster size %s", tostring(sdk.to_int64(retval))))
	--print(string.format("Lottery returned monster size %s", tostring(sdk.to_int64(retval))))
    return retval
end

-- mod.HookFunc("app.user_data.EmParamRandomSizeFish.cRandomSizeData", "lottery(System.Int32)",
-- function (args)
    -- if mod.Config.ForFish then
        -- PreHook(args)
    -- end
-- end, PostHook)

mod.HookFunc("app.user_data.EmParamRandomSize.cRandomSizeData", "lottery(System.Int32)",
PreHook, PostHook)